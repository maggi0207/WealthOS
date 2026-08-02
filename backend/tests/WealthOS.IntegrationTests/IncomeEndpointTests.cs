using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Income &amp; Business endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class IncomeEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_income_tests")
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
    public async Task CreateClient_ThenInvoice_ThenDashboard_ShouldSucceed()
    {
        var email = $"income_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Income",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createClientResponse = await _client.PostAsJsonAsync("/api/v1/clients", new CreateClientRequest
        {
            Name = "Integration Client",
            Engagement = "Retainer · API test",
            Status = ClientStatus.Active,
            MonthlyRevenue = 100_000m,
            CurrencyCode = "INR",
        });

        createClientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var clientPayload = await createClientResponse.Content.ReadFromJsonAsync<ApiResponse<ClientResponse>>();
        clientPayload!.Data.Should().NotBeNull();
        var clientId = clientPayload.Data!.Id;

        var createInvoiceResponse = await _client.PostAsJsonAsync("/api/v1/invoices", new CreateInvoiceRequest
        {
            ClientId = clientId,
            InvoiceNumber = $"INV-{Guid.NewGuid():N}".Substring(0, 20),
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 15),
            Status = InvoiceStatus.Sent,
            Items =
            [
                new CreateInvoiceItemRequest
                {
                    Description = "July retainer",
                    Quantity = 1m,
                    UnitPrice = 100_000m,
                },
            ],
        });

        createInvoiceResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoicePayload = await createInvoiceResponse.Content.ReadFromJsonAsync<ApiResponse<InvoiceResponse>>();
        invoicePayload!.Data.Should().NotBeNull();

        var paymentResponse = await _client.PostAsJsonAsync("/api/v1/payments", new RecordInvoicePaymentRequest
        {
            InvoiceId = invoicePayload.Data!.Id,
            Amount = 40_000m,
            PaidOn = new DateOnly(2026, 7, 10),
            Method = PaymentMethod.BankTransfer,
        });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var dashboardResponse = await _client.GetAsync("/api/v1/income/dashboard?period=2026-07");
        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await dashboardResponse.Content.ReadFromJsonAsync<ApiResponse<IncomeDashboardResponse>>();
        dashboard!.Data.Should().NotBeNull();
        dashboard.Data!.BusinessRevenue.Should().Be(40_000m);
        dashboard.Data.OutstandingInvoices.Should().Be(60_000m);

        var clientsResponse = await _client.GetAsync("/api/v1/clients");
        clientsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
