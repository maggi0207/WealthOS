using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;

namespace WealthOS.IntegrationTests;

/// <summary>
/// AI advisor endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class AIEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_ai_tests")
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
                builder.UseSetting("AI:DefaultProvider", "OpenAI");
                builder.UseSetting("AI:EnableToolExecution", "true");
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
    public async Task Chat_ThenHistory_ThenSuggestions_ThenInsights_ShouldSucceed()
    {
        var email = $"ai_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "AI",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var chatResponse = await _client.PostAsJsonAsync("/api/v1/ai/chat", new SendMessageRequest
        {
            Message = "Summarize my dashboard net worth",
        });

        chatResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var chatPayload = await chatResponse.Content.ReadFromJsonAsync<ApiResponse<AIChatResponse>>();
        chatPayload.Should().NotBeNull();
        chatPayload!.Success.Should().BeTrue();
        chatPayload.Data.Should().NotBeNull();
        chatPayload.Data!.IsPlaceholder.Should().BeTrue();
        chatPayload.Data.ConversationId.Should().NotBeEmpty();

        var historyResponse = await _client.GetAsync("/api/v1/ai/history");
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var suggestionsResponse = await _client.GetAsync("/api/v1/ai/suggestions");
        suggestionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var suggestionsPayload =
            await suggestionsResponse.Content.ReadFromJsonAsync<ApiResponse<AISuggestionsResponse>>();
        suggestionsPayload!.Data!.Suggestions.Should().NotBeEmpty();

        var insightsResponse = await _client.GetAsync("/api/v1/ai/insights");
        insightsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var clearResponse = await _client.DeleteAsync("/api/v1/ai/history");
        clearResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
