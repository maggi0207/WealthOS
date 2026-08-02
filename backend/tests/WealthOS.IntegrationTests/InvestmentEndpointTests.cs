using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Investments endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class InvestmentEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_investment_tests")
        .WithUsername("wealthos")
        .WithPassword("wealthos_test_password")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:DefaultConnection", _postgresContainer.GetConnectionString());
                builder.UseSetting("Jwt:Issuer", "https://tests.wealthos.local");
                builder.UseSetting("Jwt:Audience", "wealthos-tests");
                builder.UseSetting("Jwt:SecretKey", "INTEGRATION_TEST_SECRET_KEY_32_CHARS");
                builder.UseSetting("Jwt:AccessTokenExpirationMinutes", "15");
                builder.UseSetting("Jwt:RefreshTokenExpirationDays", "7");
                builder.UseSetting("AdminSeed:Email", "admin@wealthos.local");
                builder.UseSetting("AdminSeed:Password", "Admin@WealthOS1!");
            });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CreateManualAccount_ThenHolding_ThenPortfolio_ShouldSucceed()
    {
        var email = $"invest_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Invest",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var providersResponse = await _client.GetAsync("/api/v1/investments/providers");
        providersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var providersPayload = await providersResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvestmentProviderListResponse>>();
        providersPayload.Should().NotBeNull();
        providersPayload!.Success.Should().BeTrue();

        var manualProvider = providersPayload.Data!.Items.First(p => p.Kind == ProviderKind.Manual);

        var createAccountResponse = await _client.PostAsJsonAsync(
            "/api/v1/investments/accounts",
            new CreateInvestmentAccountRequest
            {
                ProviderId = manualProvider.Id,
                Name = "Test Manual",
                OwnerName = "Tester",
                KindLabel = "Manual · Test",
                Status = InvestmentAccountStatus.Manual,
                CurrencyCode = "INR",
            });

        createAccountResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var accountPayload = await createAccountResponse.Content
            .ReadFromJsonAsync<ApiResponse<InvestmentAccountResponse>>();
        accountPayload.Should().NotBeNull();
        var accountId = accountPayload!.Data!.Id;

        var addHoldingResponse = await _client.PostAsJsonAsync(
            "/api/v1/investments/manual-holding",
            new AddManualHoldingRequest
            {
                AccountId = accountId,
                Name = "Nifty Test",
                Symbol = "NIFTYTEST",
                Category = InvestmentCategory.MutualFunds,
                InvestmentType = InvestmentType.MutualFund,
                Quantity = 100,
                AverageCost = 100,
                InvestedAmount = 10_000m,
                CurrentPrice = 110,
                CurrentValue = 11_000m,
                DayChange = 100m,
                DayChangePercent = 0.91m,
                CurrencyCode = "INR",
            });

        addHoldingResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var portfolioResponse = await _client.GetAsync("/api/v1/investments/portfolio");
        portfolioResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var portfolioPayload = await portfolioResponse.Content.ReadFromJsonAsync<ApiResponse<PortfolioResponse>>();
        portfolioPayload.Should().NotBeNull();
        portfolioPayload!.Data!.CurrentValue.Should().BeGreaterThanOrEqualTo(11_000m);
        portfolioPayload.Data.HoldingCount.Should().BeGreaterThanOrEqualTo(1);
    }
}
