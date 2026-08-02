using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Common.Models;

namespace WealthOS.Application.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<Result<AuthTokensResponse>> RegisterAsync(
        RegisterRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<AuthTokensResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<AuthTokensResponse>> RefreshAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(
        LogoutRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfileResponse>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    Task<Result> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);
}
