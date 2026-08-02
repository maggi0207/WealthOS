using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.IntegrationTests;

/// <summary>
/// Documents endpoint integration skeleton. Requires Docker for Testcontainers PostgreSQL.
/// </summary>
public sealed class DocumentEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("wealthos_document_tests")
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
    public async Task CreateDocument_ThenSearch_ThenTag_ShouldSucceed()
    {
        var email = $"docs_{Guid.NewGuid():N}@wealthos.local";
        var password = "Secure@Pass1";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Doc",
            LastName = "Tester",
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<AuthTokensResponse>>();
        registerPayload.Should().NotBeNull();
        var accessToken = registerPayload!.Data!.AccessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/documents", new CreateDocumentRequest
        {
            Title = "Passport",
            Category = DocumentCategory.Identity,
            Owner = "Doc Tester",
            Status = DocumentStatus.Expiring,
            AccessLevel = DocumentAccess.Private,
            IssueDate = new DateOnly(2016, 10, 30),
            ExpiryDate = new DateOnly(2026, 10, 29),
            OriginalFileName = "passport.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 1_200_000,
            StorageProvider = DocumentStorageProvider.LocalPlaceholder,
            StoragePath = "placeholder://documents/test/passport.pdf",
            Tags = ["kyc", "travel"],
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<DocumentResponse>>();
        created.Should().NotBeNull();
        created!.Data.Should().NotBeNull();
        created.Data!.Id.Should().NotBeEmpty();
        created.Data.Title.Should().Be("Passport");

        var documentId = created.Data.Id;

        var searchResponse = await _client.GetAsync("/api/v1/documents/search?tag=kyc&category=Identity");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var search = await searchResponse.Content.ReadFromJsonAsync<ApiResponse<DocumentListResponse>>();
        search!.Data!.Items.Should().Contain(item => item.Id == documentId);

        var tagResponse = await _client.PostAsJsonAsync(
            $"/api/v1/documents/{documentId}/tags",
            new AddDocumentTagRequest { Name = "renewal" });
        tagResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var recentResponse = await _client.GetAsync("/api/v1/documents/recent?take=5");
        recentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reminderResponse = await _client.PostAsJsonAsync(
            $"/api/v1/documents/{documentId}/reminders",
            new CreateDocumentReminderRequest
            {
                ReminderDate = new DateOnly(2026, 9, 29),
                Message = "Renew passport",
            });
        reminderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
