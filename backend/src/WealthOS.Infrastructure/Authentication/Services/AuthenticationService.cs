using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.DTOs.Responses;
using WealthOS.Application.Authentication.Interfaces;
using WealthOS.Application.Authentication.Options;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.Authentication.Constants;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Authentication.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Infrastructure.Authentication.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<AuthTokensResponse>> RegisterAsync(
        RegisterRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = _userManager.NormalizeEmail(request.Email);
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Conflict("An account with this email address already exists."));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DisplayName = $"{request.FirstName.Trim()} {request.LastName.Trim()}",
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result.Failure<AuthTokensResponse>(MapIdentityErrors(createResult));
        }

        await EnsureRoleExistsAsync(AuthRoles.User, cancellationToken);
        await _userManager.AddToRoleAsync(user, AuthRoles.User);

        _logger.LogInformation("User registered successfully for {Email}", user.Email);

        return await IssueTokensAsync(user, ipAddress, cancellationToken);
    }

    public async Task<Result<AuthTokensResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive || user.IsDeleted)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Invalid email or password."));
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Invalid email or password."));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User logged in successfully for {Email}", user.Email);

        return await IssueTokensAsync(user, ipAddress, cancellationToken);
    }

    public async Task<Result<AuthTokensResponse>> RefreshAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredAccessToken(request.AccessToken);
        if (principal is null)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Invalid access token."));
        }

        var userIdValue = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Invalid access token subject."));
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || storedToken.UserId != userId)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Invalid refresh token."));
        }

        if (!storedToken.IsActive)
        {
            // Potential reuse — revoke all active tokens for the user.
            var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId, cancellationToken);
            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                _refreshTokenRepository.Update(token);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("Refresh token is no longer active."));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || !user.IsActive || user.IsDeleted)
        {
            return Result.Failure<AuthTokensResponse>(
                Error.Unauthorized("User account is not available."));
        }

        var newRefreshToken = _jwtTokenService.GenerateRefreshToken(ipAddress);
        newRefreshToken.UserId = user.Id;

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.ReplacedByToken = newRefreshToken.Token;

        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

        return Result.Success(new AuthTokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = await MapUserProfileAsync(user, roles),
        });
    }

    public async Task<Result> LogoutAsync(
        LogoutRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null)
        {
            return Result.Success();
        }

        if (storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            _refreshTokenRepository.Update(storedToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Refresh token revoked for user {UserId}", storedToken.UserId);

        return Result.Success();
    }

    public async Task<Result<UserProfileResponse>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<UserProfileResponse>(Error.Unauthorized());
        }

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId.Value.ToString());
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserProfileResponse>(Error.Unauthorized());
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result.Success(await MapUserProfileAsync(user, roles));
    }

    public async Task<Result> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        // Placeholder: no email delivery in Phase 2.
        // Always return success to avoid account enumeration.
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is not null)
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation(
                "Password reset token generated for {Email}. Email delivery is not configured. Token length: {TokenLength}",
                user.Email,
                resetToken.Length);
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return Result.Failure(Error.Failure("reset_failed", "Unable to reset password with the provided token."));
        }

        var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!resetResult.Succeeded)
        {
            return Result.Failure(MapIdentityErrors(resetResult));
        }

        // Invalidate outstanding refresh tokens after password change.
        var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = "password-reset";
            _refreshTokenRepository.Update(token);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset completed for {Email}", user.Email);

        return Result.Success();
    }

    private async Task<Result<AuthTokensResponse>> IssueTokensAsync(
        User user,
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(ipAddress);
        refreshToken.UserId = user.Id;

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = await MapUserProfileAsync(user, roles),
        });
    }

    private Task<UserProfileResponse> MapUserProfileAsync(User user, IList<string> roles)
    {
        var profile = _mapper.Map<UserProfileResponse>(user) with { Roles = roles.ToList() };
        return Task.FromResult(profile);
    }

    private async Task EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant(),
            Description = $"{roleName} role",
            CreatedAt = DateTime.UtcNow,
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // RoleManager.CreateAsync already persists; cancellationToken acknowledged for API consistency.
        _ = cancellationToken;
    }

    private static Error MapIdentityErrors(IdentityResult result)
    {
        var validationErrors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Description).ToArray());

        if (validationErrors.Count == 0)
        {
            return Error.Failure("identity_error", "Identity operation failed.");
        }

        return Error.Validation("Identity validation failed.", validationErrors);
    }
}
