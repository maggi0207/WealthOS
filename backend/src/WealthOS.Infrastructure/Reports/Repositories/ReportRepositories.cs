using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Models;
using WealthOS.Domain.Reports.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Reports.Repositories;

public sealed class ReportDefinitionRepository : Repository<ReportDefinition>, IReportDefinitionRepository
{
    public ReportDefinitionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ReportDefinition?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(definition => definition.Code == code, cancellationToken);

    public async Task<ReportDefinition?> GetByTypeAsync(
        ReportType reportType,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(
                definition =>
                    definition.ReportType == reportType &&
                    definition.Status == ReportDefinitionStatus.Active,
                cancellationToken);

    public async Task<IReadOnlyList<ReportDefinitionSummary>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(definition => definition.Status == ReportDefinitionStatus.Active)
            .OrderBy(definition => definition.SortOrder)
            .Select(definition => new ReportDefinitionSummary
            {
                Id = definition.Id,
                Code = definition.Code,
                Name = definition.Name,
                ReportType = definition.ReportType,
                Status = definition.Status,
            })
            .ToListAsync(cancellationToken);
}

public sealed class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Report?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            report => report.Id == id && report.UserId == userId,
            cancellationToken);

    public async Task<(IReadOnlyList<Report> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(report => report.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(report => report.LastGeneratedAt ?? report.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public sealed class ReportExecutionRepository : Repository<ReportExecution>, IReportExecutionRepository
{
    public ReportExecutionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<ReportExecution> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(execution => execution.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(execution => execution.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public sealed class ReportSnapshotRepository : Repository<ReportSnapshot>, IReportSnapshotRepository
{
    public ReportSnapshotRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ReportSnapshot?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            snapshot => snapshot.Id == id && snapshot.UserId == userId,
            cancellationToken);

    public async Task<(IReadOnlyList<ReportSnapshot> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        ReportType? reportType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(snapshot => snapshot.UserId == userId);
        if (reportType.HasValue)
        {
            query = query.Where(snapshot => snapshot.ReportType == reportType.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(snapshot => snapshot.CapturedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public sealed class ReportExportRepository : Repository<ReportExport>, IReportExportRepository
{
    public ReportExportRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<ReportExport> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(export => export.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(export => export.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
