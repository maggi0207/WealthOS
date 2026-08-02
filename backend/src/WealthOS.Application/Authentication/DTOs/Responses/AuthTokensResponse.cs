namespace WealthOS.Application.Authentication.DTOs.Responses;

public sealed class AuthTokensResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public UserProfileResponse User { get; init; } = null!;
}
