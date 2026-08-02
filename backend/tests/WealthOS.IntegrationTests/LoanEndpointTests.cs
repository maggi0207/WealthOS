using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Loans endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class LoanEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_loan_tests")
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
    public async Task Create_ThenGet_ThenSummary_ShouldSucceed()
    {
        var email = $"loan_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Loan",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/loans", new CreateLoanRequest
        {
            Name = "Integration personal loan",
            Type = LoanType.Personal,
            LenderName = "Test Bank",
            Principal = 200_000m,
            OutstandingBalance = 150_000m,
            InterestRate = 12m,
            InterestType = InterestType.Fixed,
            EmiAmount = 8_000m,
            TenureMonths = 36,
            RemainingTenureMonths = 24,
            StartDate = new DateOnly(2025, 1, 1),
            NextEmiDate = new DateOnly(2026, 8, 10),
            CurrencyCode = "INR",
            Status = LoanStatus.Active,
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<LoanResponse>>();
        created.Should().NotBeNull();
        created!.Success.Should().BeTrue();
        created.Data.Should().NotBeNull();
        var loanId = created.Data!.Id;

        var getResponse = await _client.GetAsync($"/api/v1/loans/{loanId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var summaryResponse = await _client.GetAsync("/api/v1/loans/summary");
        summaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await summaryResponse.Content.ReadFromJsonAsync<ApiResponse<LoanSummaryResponse>>();
        summary!.Data!.LoanCount.Should().BeGreaterThanOrEqualTo(1);
        summary.Data.OutstandingBalance.Should().BeGreaterThan(0);

        var upcomingResponse = await _client.GetAsync("/api/v1/loans/upcoming");
        upcomingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboardResponse = await _client.GetAsync($"/api/v1/loans/{loanId}/dashboard");
        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
