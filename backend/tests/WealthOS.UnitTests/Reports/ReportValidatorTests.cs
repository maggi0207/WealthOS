using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.Validators;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.UnitTests.Reports;

/// <summary>
/// Validator coverage for Reports &amp; Analytics request DTOs.
/// </summary>
public sealed class ReportValidatorTests
{
    private readonly ReportFilterRequestValidator _filterValidator = new();
    private readonly GenerateSnapshotRequestValidator _snapshotValidator = new();
    private readonly ExportReportRequestValidator _exportValidator = new();

    [Fact]
    public void Filter_WhenFromAfterTo_ShouldFail()
    {
        var result = _filterValidator.TestValidate(new ReportFilterRequest
        {
            FromDate = DateTime.UtcNow,
            ToDate = DateTime.UtcNow.AddDays(-1),
        });

        result.ShouldHaveValidationErrorFor("FromDate");
    }

    [Fact]
    public void Filter_WhenValid_ShouldPass()
    {
        var result = _filterValidator.TestValidate(new ReportFilterRequest
        {
            FromDate = DateTime.UtcNow.AddMonths(-1),
            ToDate = DateTime.UtcNow,
            Period = AnalyticsPeriod.Monthly,
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Snapshot_WhenTypeInvalid_ShouldFail()
    {
        var result = _snapshotValidator.TestValidate(new GenerateSnapshotRequest
        {
            ReportType = (ReportType)999,
        });

        result.ShouldHaveValidationErrorFor(request => request.ReportType);
    }

    [Fact]
    public void Export_WhenValid_ShouldPass()
    {
        var result = _exportValidator.TestValidate(new ExportReportRequest
        {
            ReportType = ReportType.NetWorth,
            Format = ReportExportFormat.Json,
        });

        result.IsValid.Should().BeTrue();
    }
}
