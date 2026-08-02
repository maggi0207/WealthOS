using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Reports.Commands;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;

namespace WealthOS.Application.Reports.Commands.Handlers;

public sealed class GenerateSnapshotCommandHandler
    : ICommandHandler<GenerateSnapshotCommand, ReportSnapshotResponse>
{
    private readonly IReportService _reportService;

    public GenerateSnapshotCommandHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<ReportSnapshotResponse>> HandleAsync(
        GenerateSnapshotCommand command,
        CancellationToken cancellationToken = default) =>
        _reportService.GenerateSnapshotAsync(command.Request, cancellationToken);
}

public sealed class ExportReportCommandHandler
    : ICommandHandler<ExportReportCommand, ReportExportResponse>
{
    private readonly IExportService _exportService;

    public ExportReportCommandHandler(IExportService exportService) =>
        _exportService = exportService;

    public Task<Result<ReportExportResponse>> HandleAsync(
        ExportReportCommand command,
        CancellationToken cancellationToken = default) =>
        _exportService.ExportAsync(command.Request, cancellationToken);
}
