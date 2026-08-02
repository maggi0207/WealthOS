namespace WealthOS.Application.Authentication.DTOs.Requests;

public sealed class ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
}
