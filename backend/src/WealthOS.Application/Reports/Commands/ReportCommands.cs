using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Reports.DTOs.Requests;

namespace WealthOS.Application.Reports.Commands;

/// <summary>Captures a point-in-time report snapshot.</summary>
public sealed class GenerateSnapshotCommand : ICommand
{
    public GenerateSnapshotRequest Request { get; init; } = null!;
}

/// <summary>Requests a report export (architecture placeholder).</summary>
public sealed class ExportReportCommand : ICommand
{
    public ExportReportRequest Request { get; init; } = null!;
}
