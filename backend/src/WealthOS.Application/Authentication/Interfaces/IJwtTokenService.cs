using System.Security.Claims;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Application.Authentication.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);

    RefreshToken GenerateRefreshToken(string createdByIp);

    ClaimsPrincipal? GetPrincipalFromExpiredAccessToken(string accessToken);
}
