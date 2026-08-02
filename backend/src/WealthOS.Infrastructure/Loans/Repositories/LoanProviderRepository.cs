using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Loans.Repositories;

/// <summary>
/// EF Core repository for loan providers (lenders).
/// </summary>
public sealed class LoanProviderRepository : Repository<LoanProvider>, ILoanProviderRepository
{
    public LoanProviderRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<LoanProvider?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            provider => provider.Id == id && provider.UserId == userId,
            cancellationToken);

    public async Task<LoanProvider?> FindByNameForUserAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return await DbSet.FirstOrDefaultAsync(
            provider => provider.UserId == userId
                && provider.Name.ToLower() == normalized,
            cancellationToken);
    }
}
