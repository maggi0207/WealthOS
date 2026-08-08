using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Settings.Entities;
using WealthOS.Domain.Settings.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Settings.Repositories;

public sealed class UserSettingsRepository : Repository<UserSettings>, IUserSettingsRepository
{
    public UserSettingsRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<UserSettings?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(settings => settings.UserId == userId, cancellationToken);
}
