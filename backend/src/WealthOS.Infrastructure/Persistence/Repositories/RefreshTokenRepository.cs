using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Authentication.Repositories;

namespace WealthOS.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        await _context.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.Token == token, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == userId
                && refreshToken.RevokedAt == null
                && refreshToken.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) =>
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public void Update(RefreshToken refreshToken) => _context.RefreshTokens.Update(refreshToken);

    public void Remove(RefreshToken refreshToken) => _context.RefreshTokens.Remove(refreshToken);
}
