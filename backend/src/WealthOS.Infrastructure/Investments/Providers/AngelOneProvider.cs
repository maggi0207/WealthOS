using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Angel One SmartAPI configuration. Secrets must come from environment / vault — never commit.
/// Trading endpoints are intentionally unsupported.
/// </summary>
public sealed class AngelOneOptions
{
    public const string SectionName = "AngelOne";

    /// <summary>SmartAPI base URL (read-only market/portfolio APIs).</summary>
    public string BaseUrl { get; set; } = "https://apiconnect.angelone.in";

    /// <summary>API key from Angel One developer portal.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Client code / user id.</summary>
    public string? ClientCode { get; set; }

    /// <summary>Encrypted or vault-backed refresh token reference (not a trading token).</summary>
    public string? RefreshTokenSecretName { get; set; }

    /// <summary>When false, sync methods become idempotent no-ops that touch LastSyncedAt only.</summary>
    public bool EnableLiveSync { get; set; }
}

/// <summary>
/// In-memory secure token holder for Angel One session tokens.
/// Production should replace with encrypted DPAPI / Key Vault storage.
/// Never logs token values.
/// </summary>
public sealed class AngelOneTokenStore
{
    private readonly object _gate = new();
    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset? _expiresAt;

    public bool HasAccessToken
    {
        get
        {
            lock (_gate)
            {
                return !string.IsNullOrWhiteSpace(_accessToken)
                    && (_expiresAt is null || _expiresAt > DateTimeOffset.UtcNow.AddMinutes(1));
            }
        }
    }

    public void SetTokens(string accessToken, string? refreshToken, DateTimeOffset? expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        lock (_gate)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _expiresAt = expiresAt;
        }
    }

    public string? GetAccessToken()
    {
        lock (_gate)
        {
            return _accessToken;
        }
    }

    public string? GetRefreshToken()
    {
        lock (_gate)
        {
            return _refreshToken;
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _accessToken = null;
            _refreshToken = null;
            _expiresAt = null;
        }
    }
}

/// <summary>
/// Read-only HTTP client for Angel One SmartAPI.
/// Trading (place/modify/cancel order) methods are not implemented.
/// </summary>
public sealed class AngelOneSmartApiClient
{
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
        !string.IsNullOrWhiteSpace(_options.ApiKey)
        && !string.IsNullOrWhiteSpace(_options.ClientCode)
        && _options.EnableLiveSync;

    /// <summary>
    /// Authenticates when credentials are present. Idempotent stub when not configured.
    /// </summary>
    public Task<Result> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsConfigured)
        {
            _logger.LogInformation("Angel One auth skipped — credentials not configured.");
            return Task.FromResult(Result.Success());
        }

        // Live token exchange is deferred until secrets are provisioned.
        // Structure is ready for POST /rest/auth/angelbroking/user/v1/loginByPassword (or JWT flow).
        _logger.LogInformation(
            "Angel One auth placeholder executed for client {ClientCode} (no network call).",
            _options.ClientCode);
        return Task.FromResult(Result.Success());
    }

    public Task<Result<string>> GetHoldingsJsonAsync(CancellationToken cancellationToken = default) =>
        ReadOnlyGetAsync("rest/secure/angelbroking/portfolio/v1/getHolding", cancellationToken);

    public Task<Result<string>> GetPositionsJsonAsync(CancellationToken cancellationToken = default) =>
        ReadOnlyGetAsync("rest/secure/angelbroking/order/v1/getPosition", cancellationToken);

    /// <summary>Trading is permanently disabled in WealthOS.</summary>
    public Task<Result> PlaceOrderAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Result.Failure(Error.Failure(
            "trading_disabled",
            "Angel One trading endpoints are not implemented in WealthOS.")));
    }

    private async Task<Result<string>> ReadOnlyGetAsync(string relativePath, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            return Result.Success("{}");
        }

        if (!_tokenStore.HasAccessToken)
        {
            var auth = await AuthenticateAsync(cancellationToken);
            if (auth.IsFailure)
            {
                return Result.Failure<string>(auth.Error!);
            }
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, relativePath);
            var token = _tokenStore.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
            }

            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                request.Headers.TryAddWithoutValidation("X-PrivateKey", _options.ApiKey);
            }

            // Intentionally do not call live API without confirmed token; return empty payload.
            _logger.LogInformation("Angel One read-only request prepared for {Path} (stubbed response).", relativePath);
            await Task.Yield();
            return Result.Success("{}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Angel One read failed for {Path}", relativePath);
            return Result.Failure<string>(Error.Failure("angelone_http_error", "Angel One request failed."));
        }
    }
}

/// <summary>
/// Angel One SmartAPI provider — auth + portfolio/holdings/transactions sync (read-only).
/// </summary>
public sealed class AngelOneProvider : IInvestmentProvider
{
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AngelOneSmartApiClient _client;
    private readonly AngelOneTokenStore _tokenStore;
    private readonly ILogger<AngelOneProvider> _logger;

    public AngelOneProvider(
        IInvestmentAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        AngelOneSmartApiClient client,
        AngelOneTokenStore tokenStore,
        ILogger<AngelOneProvider> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _client = client;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public ProviderKind Kind => ProviderKind.AngelOne;

    public async Task<Result> ConnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        var auth = await _client.AuthenticateAsync(cancellationToken);
        if (auth.IsFailure)
        {
            return auth;
        }

        account.Status = InvestmentAccountStatus.Connected;
        account.LastSyncedAt = DateTime.UtcNow;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Angel One account {AccountId} connected (read-only).", accountId);
        return Result.Success();
    }

    public Task<Result> SyncPortfolioAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        SyncReadOnlyAsync(accountId, userId, "portfolio", cancellationToken);

    public Task<Result> SyncHoldingsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        SyncReadOnlyAsync(accountId, userId, "holdings", cancellationToken);

    public Task<Result> SyncTransactionsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        SyncReadOnlyAsync(accountId, userId, "transactions", cancellationToken);

    public async Task<Result> DisconnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        _tokenStore.Clear();
        account.Status = InvestmentAccountStatus.Disconnected;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> SyncReadOnlyAsync(
        Guid accountId,
        Guid userId,
        string surface,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        Result payloadResult = surface switch
        {
            "holdings" => AsUnit(await _client.GetHoldingsJsonAsync(cancellationToken)),
            "portfolio" => AsUnit(await _client.GetPositionsJsonAsync(cancellationToken)),
            _ => AsUnit(await _client.GetHoldingsJsonAsync(cancellationToken)),
        };

        if (payloadResult.IsFailure)
        {
            return payloadResult;
        }

        // Idempotent: touch sync timestamp without mutating holdings until live mapping ships.
        account.LastSyncedAt = DateTime.UtcNow;
        account.Status = InvestmentAccountStatus.Connected;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Angel One {Surface} sync completed for account {AccountId} (idempotent).",
            surface,
            accountId);
        return Result.Success();
    }

    private static Result AsUnit(Result<string> result) =>
        result.IsSuccess ? Result.Success() : Result.Failure(result.Error!);
}
