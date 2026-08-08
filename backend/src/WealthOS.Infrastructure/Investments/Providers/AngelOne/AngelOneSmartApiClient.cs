using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Common.Models;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Read-only HTTP client for Angel One SmartAPI.
/// Trading (place/modify/cancel order) methods are not implemented.
/// </summary>
public sealed class AngelOneSmartApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly AngelOneOptions _options;
    private readonly AngelOneTokenStore _tokenStore;
    private readonly ILogger<AngelOneSmartApiClient> _logger;

    public AngelOneSmartApiClient(
        HttpClient httpClient,
        IOptions<AngelOneOptions> options,
        AngelOneTokenStore tokenStore,
        ILogger<AngelOneSmartApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _tokenStore = tokenStore;
        _logger = logger;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        }
    }

    public bool IsConfigured =>
        _options.EnableLiveSync
        && !string.IsNullOrWhiteSpace(_options.ApiKey)
        && !string.IsNullOrWhiteSpace(_options.ClientCode)
        && !string.IsNullOrWhiteSpace(_options.Password)
        && !string.IsNullOrWhiteSpace(_options.TotpSecret);

    public async Task<Result> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_options.EnableLiveSync)
        {
            _logger.LogInformation("Angel One auth skipped — EnableLiveSync is false.");
            return Result.Success();
        }

        if (!IsConfigured)
        {
            return Result.Failure(Error.Failure(
                "angelone_not_configured",
                "Angel One live sync requires ApiKey, ClientCode, Password (PIN), TotpSecret, and EnableLiveSync=true."));
        }

        if (_tokenStore.HasAccessToken)
        {
            return Result.Success();
        }

        var refresh = _tokenStore.GetRefreshToken();
        if (!string.IsNullOrWhiteSpace(refresh))
        {
            var refreshed = await GenerateTokensAsync(refresh, cancellationToken);
            if (refreshed.IsSuccess)
            {
                return Result.Success();
            }

            _logger.LogWarning("Angel One refresh failed; falling back to loginByPassword.");
        }

        return await LoginByPasswordAsync(cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AngelOneHoldingDto>>> GetAllHoldingsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_options.EnableLiveSync)
        {
            return Result.Success<IReadOnlyList<AngelOneHoldingDto>>([]);
        }

        var auth = await AuthenticateAsync(cancellationToken);
        if (auth.IsFailure)
        {
            return Result.Failure<IReadOnlyList<AngelOneHoldingDto>>(auth.Error!);
        }

        var jsonResult = await SendAsync(
            HttpMethod.Get,
            "rest/secure/angelbroking/portfolio/v1/getAllHolding",
            body: null,
            includeAuth: true,
            cancellationToken);

        if (jsonResult.IsFailure)
        {
            // Fallback to legacy getHolding if getAllHolding is unavailable.
            jsonResult = await SendAsync(
                HttpMethod.Get,
                "rest/secure/angelbroking/portfolio/v1/getHolding",
                body: null,
                includeAuth: true,
                cancellationToken);
            if (jsonResult.IsFailure)
            {
                return Result.Failure<IReadOnlyList<AngelOneHoldingDto>>(jsonResult.Error!);
            }
        }

        try
        {
            var holdings = AngelOneHoldingDto.ParseAllHoldingsPayload(jsonResult.Value);
            return Result.Success(holdings);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse Angel One holdings payload.");
            return Result.Failure<IReadOnlyList<AngelOneHoldingDto>>(Error.Failure(
                "angelone_parse_error",
                "Angel One holdings response could not be parsed."));
        }
    }

    /// <summary>Trading is permanently disabled in WealthOS.</summary>
    public Task<Result> PlaceOrderAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Result.Failure(Error.Failure(
            "trading_disabled",
            "Angel One trading endpoints are not implemented in WealthOS.")));
    }

    private async Task<Result> LoginByPasswordAsync(CancellationToken cancellationToken)
    {
        string totp;
        try
        {
            totp = AngelOneTotp.Generate(_options.TotpSecret!, DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid Angel One TOTP secret.");
            return Result.Failure(Error.Failure(
                "angelone_totp_invalid",
                "Angel One TotpSecret is invalid. Use the Base32 secret from SmartAPI Enable TOTP."));
        }

        var payload = JsonSerializer.Serialize(new
        {
            clientcode = _options.ClientCode,
            password = _options.Password,
            totp,
            state = "wealthos",
        });

        var jsonResult = await SendAsync(
            HttpMethod.Post,
            "rest/auth/angelbroking/user/v1/loginByPassword",
            payload,
            includeAuth: false,
            cancellationToken);

        if (jsonResult.IsFailure)
        {
            return Result.Failure(jsonResult.Error!);
        }

        return StoreTokensFromLoginPayload(jsonResult.Value, "loginByPassword");
    }

    private async Task<Result> GenerateTokensAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new { refreshToken });
        var jsonResult = await SendAsync(
            HttpMethod.Post,
            "rest/auth/angelbroking/jwt/v1/generateTokens",
            payload,
            includeAuth: false,
            cancellationToken);

        if (jsonResult.IsFailure)
        {
            return Result.Failure(jsonResult.Error!);
        }

        return StoreTokensFromLoginPayload(jsonResult.Value, "generateTokens");
    }

    private Result StoreTokensFromLoginPayload(string json, string source)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<AngelOneApiEnvelope>(json, JsonOptions);
            if (envelope is null || !envelope.Status)
            {
                var message = envelope?.Message ?? "Angel One authentication failed.";
                _logger.LogWarning(
                    "Angel One {Source} failed. ErrorCode={ErrorCode} Message={Message}",
                    source,
                    envelope?.ErrorCode,
                    message);
                return Result.Failure(Error.Failure("angelone_auth_failed", message));
            }

            var data = envelope.Data.Deserialize<AngelOneLoginData>(JsonOptions);
            if (string.IsNullOrWhiteSpace(data?.JwtToken))
            {
                return Result.Failure(Error.Failure(
                    "angelone_auth_failed",
                    "Angel One login response did not include a JWT token."));
            }

            // SmartAPI sessions typically remain valid until midnight (IST).
            var ist = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "India Standard Time" : "Asia/Kolkata");
            var nowIst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ist);
            var midnightIst = new DateTimeOffset(nowIst.Date.AddDays(1), nowIst.Offset);
            var expiresUtc = midnightIst.ToUniversalTime();

            _tokenStore.SetTokens(data.JwtToken, data.RefreshToken, expiresUtc);
            _logger.LogInformation("Angel One {Source} succeeded for client {ClientCode}.", source, _options.ClientCode);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Angel One {Source} response parse failed.", source);
            return Result.Failure(Error.Failure(
                "angelone_auth_failed",
                "Angel One authentication response could not be parsed."));
        }
    }

    private async Task<Result<string>> SendAsync(
        HttpMethod method,
        string relativePath,
        string? body,
        bool includeAuth,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(method, relativePath);
            ApplyCommonHeaders(request, includeAuth);
            if (body is not null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Angel One HTTP {StatusCode} for {Path}",
                    (int)response.StatusCode,
                    relativePath);
                return Result.Failure<string>(Error.Failure(
                    "angelone_http_error",
                    $"Angel One request failed with HTTP {(int)response.StatusCode}."));
            }

            return Result.Success(content);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Angel One request failed for {Path}", relativePath);
            return Result.Failure<string>(Error.Failure(
                "angelone_http_error",
                "Angel One request failed."));
        }
    }

    private void ApplyCommonHeaders(HttpRequestMessage request, bool includeAuth)
    {
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("X-UserType", "USER");
        request.Headers.TryAddWithoutValidation("X-SourceID", "WEB");
        request.Headers.TryAddWithoutValidation("X-ClientLocalIP", _options.ClientLocalIp);
        request.Headers.TryAddWithoutValidation("X-ClientPublicIP", _options.ClientPublicIp);
        request.Headers.TryAddWithoutValidation("X-MACAddress", _options.MacAddress);
        request.Headers.TryAddWithoutValidation("X-PrivateKey", _options.ApiKey);

        if (!includeAuth)
        {
            return;
        }

        var token = _tokenStore.GetAccessToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        }
    }
}
