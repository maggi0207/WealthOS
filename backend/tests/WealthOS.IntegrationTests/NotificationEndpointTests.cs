using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Notifications endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class NotificationEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_notification_tests")
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
    public async Task CreateNotification_ThenMarkRead_ThenCreateReminder_ShouldSucceed()
    {
        var email = $"notify_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Notify",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/notifications", new CreateNotificationRequest
        {
            Title = "SIP due",
            Message = "Monthly SIP reminder",
            Type = NotificationType.SipReminder,
            Channel = NotificationChannel.InApp,
            Priority = NotificationPriority.Normal,
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NotificationResponse>>();
        created.Should().NotBeNull();
        created!.Success.Should().BeTrue();
        created.Data.Should().NotBeNull();
        var notificationId = created.Data!.Id;

        var unreadResponse = await _client.GetAsync("/api/v1/notifications/unread");
        unreadResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var markReadResponse = await _client.PutAsync($"/api/v1/notifications/{notificationId}/read", null);
        markReadResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reminderResponse = await _client.PostAsJsonAsync("/api/v1/reminders", new CreateReminderRequest
        {
            Title = "Bond maturity",
            Message = "Check bond maturity next week",
            ReminderType = NotificationType.BondMaturity,
            DueAt = DateTime.UtcNow.AddDays(7),
        });

        reminderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var reminder = await reminderResponse.Content.ReadFromJsonAsync<ApiResponse<ReminderResponse>>();
        reminder.Should().NotBeNull();
        reminder!.Success.Should().BeTrue();
        reminder.Data!.Title.Should().Be("Bond maturity");

        var preferencesResponse = await _client.PutAsJsonAsync(
            "/api/v1/notifications/preferences",
            new UpdateNotificationPreferencesRequest
            {
                Preferences =
                [
                    new NotificationPreferenceItemRequest
                    {
                        NotificationType = NotificationType.SipReminder,
                        EnableInApp = true,
                        EnableEmail = false,
                    },
                ],
            });

        preferencesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
