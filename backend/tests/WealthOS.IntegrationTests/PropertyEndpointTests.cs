using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Properties endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class PropertyEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_property_tests")
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
        var email = $"prop_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Prop",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/properties", new CreatePropertyRequest
        {
            Name = "Test Apartment",
            Type = PropertyType.Apartment,
            OwnershipType = OwnershipType.Sole,
            PurchasePrice = 1_000_000m,
            CurrentMarketValue = 1_200_000m,
            CurrencyCode = "INR",
            Status = PropertyStatus.Active,
            Address = new PropertyAddressRequest
            {
                City = "Chennai",
                Locality = "Adyar",
            },
            Owners =
            [
                new PropertyOwnerRequest
                {
                    Name = "Prop Tester",
                    OwnershipPercentage = 100m,
                    IsPrimary = true,
                },
            ],
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createPayload = await createResponse.Content.ReadFromJsonAsync<ApiResponse<PropertyResponse>>();
        createPayload.Should().NotBeNull();
        createPayload!.Success.Should().BeTrue();
        createPayload.Data.Should().NotBeNull();
        createPayload.Data!.Name.Should().Be("Test Apartment");

        var propertyId = createPayload.Data.Id;

        var getResponse = await _client.GetAsync($"/api/v1/properties/{propertyId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var summaryResponse = await _client.GetAsync("/api/v1/properties/summary");
        summaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var summaryPayload = await summaryResponse.Content.ReadFromJsonAsync<ApiResponse<PropertySummaryResponse>>();
        summaryPayload.Should().NotBeNull();
        summaryPayload!.Data!.PropertyCount.Should().BeGreaterThanOrEqualTo(1);

        var dashboardResponse = await _client.GetAsync($"/api/v1/properties/{propertyId}/dashboard");
        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/properties/{propertyId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAfterDelete = await _client.GetAsync($"/api/v1/properties/{propertyId}");
        getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/properties");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
