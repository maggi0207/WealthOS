using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Repositories;

namespace WealthOS.Application.Reports.Services;

/// <summary>
/// Export architecture stub. Persists metadata only; does not generate CSV/Excel/PDF files.
/// </summary>
public sealed class ExportService : IExportService
{
    private static readonly ReportExportFormat[] Supported =
    [
        ReportExportFormat.Csv,
        ReportExportFormat.Excel,
        ReportExportFormat.Pdf,
        ReportExportFormat.Json,
    ];

    private readonly IReportExportRepository _exportRepository;
    private readonly IReportSnapshotRepository _snapshotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ExportService(
        IReportExportRepository exportRepository,
        IReportSnapshotRepository snapshotRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _exportRepository = exportRepository;
        _snapshotRepository = snapshotRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public IReadOnlyList<ReportExportFormat> GetSupportedFormats() => Supported;

    public async Task<Result<ReportExportResponse>> ExportAsync(
        ExportReportRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<ReportExportResponse>(Error.Unauthorized());
        }

        if (!Supported.Contains(request.Format))
        {
            return Result.Failure<ReportExportResponse>(
                Error.Validation(
                    "Unsupported export format.",
                    new Dictionary<string, string[]>
                    {
                        ["Format"] = ["Format must be Csv, Excel, Pdf, or Json."],
                    }));
        }

        if (request.SnapshotId is Guid snapshotId)
        {
            var snapshot = await _snapshotRepository.GetByIdForUserAsync(
                snapshotId,
                _currentUser.UserId.Value,
                cancellationToken);
            if (snapshot is null)
            {
                return Result.Failure<ReportExportResponse>(
                    Error.NotFound("ReportSnapshot", snapshotId));
            }
        }

        var (fileName, contentType) = DescribeFormat(request.Format, request.ReportType);
        var export = new ReportExport
        {
            UserId = _currentUser.UserId.Value,
            ReportSnapshotId = request.SnapshotId,
            ReportType = request.ReportType,
            Format = request.Format,
            Status = ReportExportStatus.NotImplemented,
            FileName = fileName,
            ContentType = contentType,
            Message =
                $"Export to {request.Format} is not implemented in Phase 12. " +
                "Architecture placeholder only — no file generation libraries are wired.",
            RequestedAt = DateTime.UtcNow,
        };

        await _exportRepository.AddAsync(export, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ReportExportResponse
        {
            Id = export.Id,
            ReportType = export.ReportType,
            Format = export.Format,
            Status = export.Status,
            Message = export.Message ?? string.Empty,
            FileName = export.FileName,
            ContentType = export.ContentType,
            RequestedAt = export.RequestedAt,
            SupportedFormats = Supported.Select(format => format.ToString()).ToList(),
        });
    }

    private static (string FileName, string ContentType) DescribeFormat(
        ReportExportFormat format,
        ReportType reportType)
    {
        var baseName = $"wealthos-{reportType.ToString().ToLowerInvariant()}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        return format switch
        {
            ReportExportFormat.Csv => ($"{baseName}.csv", "text/csv"),
            ReportExportFormat.Excel => ($"{baseName}.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            ReportExportFormat.Pdf => ($"{baseName}.pdf", "application/pdf"),
            _ => ($"{baseName}.json", "application/json"),
        };
    }
}
