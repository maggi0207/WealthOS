using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Models;

namespace WealthOS.Domain.Reports.Repositories;

/// <summary>Persistence for report catalog definitions.</summary>
public interface IReportDefinitionRepository : IRepository<ReportDefinition>
{
    Task<ReportDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<ReportDefinition?> GetByTypeAsync(ReportType reportType, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportDefinitionSummary>> ListActiveAsync(CancellationToken cancellationToken = default);
}

/// <summary>Persistence for user report instances.</summary>
public interface IReportRepository : IRepository<Report>
{
    Task<Report?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Report> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for report execution audits.</summary>
public interface IReportExecutionRepository : IRepository<ReportExecution>
{
    Task<(IReadOnlyList<ReportExecution> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for report snapshots.</summary>
public interface IReportSnapshotRepository : IRepository<ReportSnapshot>
{
    Task<ReportSnapshot?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ReportSnapshot> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        ReportType? reportType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for export request metadata.</summary>
public interface IReportExportRepository : IRepository<ReportExport>
{
    Task<(IReadOnlyList<ReportExport> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
