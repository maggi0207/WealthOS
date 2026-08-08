using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Settings.Entities;

namespace WealthOS.Domain.Settings.Repositories;

public interface IUserSettingsRepository : IRepository<UserSettings>
{
    Task<UserSettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
