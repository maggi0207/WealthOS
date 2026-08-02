namespace WealthOS.Application.Authentication.DTOs.Requests;

public sealed class LogoutRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
