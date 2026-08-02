using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WealthOS.Application.Common.DTOs;

namespace WealthOS.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<HealthReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HealthReportDto>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken: cancellationToken);

        var data = new HealthReportDto
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration,
            Entries = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthEntryDto
                {
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    Duration = entry.Value.Duration,
                }),
        };

        var response = ApiResponse<HealthReportDto>.Ok(data, "Health check completed.");

        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}

public sealed class HealthReportDto
{
    public string Status { get; init; } = string.Empty;

    public TimeSpan TotalDuration { get; init; }

    public Dictionary<string, HealthEntryDto> Entries { get; init; } = new();
}

public sealed class HealthEntryDto
{
    public string Status { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TimeSpan Duration { get; init; }
}
