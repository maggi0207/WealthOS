using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Goals endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class GoalEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_goal_tests")
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
    public async Task CreateGoal_ThenContribute_ThenDashboard_ShouldSucceed()
    {
        var email = $"goals_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Goal",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/goals", new CreateGoalRequest
        {
            Name = "Vacation fund",
            Category = GoalCategory.Vacation,
            TargetAmount = 2_00_000m,
            CurrentAmount = 50_000m,
            MonthlyContribution = 10_000m,
            TargetDate = new DateOnly(2028, 12, 1),
            StartedOn = new DateOnly(2026, 1, 1),
            Priority = GoalPriority.Medium,
            Status = GoalStatus.Active,
            CurrencyCode = "INR",
            Description = "Family trip corpus",
            Milestones =
            [
                new CreateGoalMilestoneRequest
                {
                    Label = "Half funded",
                    TargetPercent = 50m,
                    SortOrder = 0,
                },
                new CreateGoalMilestoneRequest
                {
                    Label = "Fully funded",
                    TargetPercent = 100m,
                    SortOrder = 1,
                },
            ],
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<GoalResponse>>();
        created.Should().NotBeNull();
        created!.Data.Should().NotBeNull();
        created.Data!.Id.Should().NotBeEmpty();
        created.Data.CompletionPercent.Should().Be(25m);

        var goalId = created.Data.Id;

        var contributionResponse = await _client.PostAsJsonAsync(
            $"/api/v1/goals/{goalId}/contributions",
            new RecordGoalContributionRequest
            {
                Amount = 25_000m,
                ContributedOn = new DateOnly(2026, 8, 1),
                Source = "Bonus",
            });

        contributionResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var progressResponse = await _client.GetAsync($"/api/v1/goals/{goalId}/progress");
        progressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var progress = await progressResponse.Content.ReadFromJsonAsync<ApiResponse<GoalProgressResponse>>();
        progress!.Data!.CurrentAmount.Should().Be(75_000m);

        var dashboardResponse = await _client.GetAsync("/api/v1/goals/dashboard");
        dashboardResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await dashboardResponse.Content.ReadFromJsonAsync<ApiResponse<GoalDashboardResponse>>();
        dashboard!.Data!.ActiveGoals.Should().BeGreaterThanOrEqualTo(1);
        dashboard.Data.TotalGoalValue.Should().BeGreaterThanOrEqualTo(2_00_000m);

        var projectionResponse = await _client.GetAsync($"/api/v1/goals/{goalId}/projection");
        projectionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
