using Microsoft.AspNetCore.Identity;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Infrastructure.Authentication.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string Hash(string password) => _passwordHasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
