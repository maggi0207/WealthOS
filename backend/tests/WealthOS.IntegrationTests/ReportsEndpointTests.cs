using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Reports endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class ReportsEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_reports_tests")
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
    public async Task Reports_GetNetWorth_ThenSummary_ThenExport_ShouldSucceed()
    {
        var email = $"reports_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Reports",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var netWorthResponse = await _client.GetAsync("/api/v1/reports/networth");
        netWorthResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var netWorthPayload = await netWorthResponse.Content.ReadFromJsonAsync<ApiResponse<NetWorthReportResponse>>();
        netWorthPayload!.Success.Should().BeTrue();
        netWorthPayload.Data.Should().NotBeNull();

        var summaryResponse = await _client.GetAsync("/api/v1/reports/summary");
        summaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var healthResponse = await _client.GetAsync("/api/v1/reports/financial-health");
        healthResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var exportResponse = await _client.PostAsJsonAsync("/api/v1/reports/exports", new ExportReportRequest
        {
            ReportType = ReportType.NetWorth,
            Format = ReportExportFormat.Json,
        });

        exportResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var exportPayload = await exportResponse.Content.ReadFromJsonAsync<ApiResponse<ReportExportResponse>>();
        exportPayload!.Data!.Status.Should().Be(ReportExportStatus.NotImplemented);
    }
}
