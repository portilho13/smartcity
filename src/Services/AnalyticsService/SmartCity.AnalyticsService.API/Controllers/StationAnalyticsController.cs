using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.AnalyticsService.Core.DTOs;
using SmartCity.AnalyticsService.Core.Services;

namespace SmartCity.AnalyticsService.API.Controllers;

[ApiController]
[Route("api/v1/analytics/[controller]")]
[Authorize(Roles = "admin")]
public class StationsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<StationsController> _logger;

    public StationsController(IAnalyticsService analyticsService, ILogger<StationsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get analytics for all stations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StationAnalyticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStationAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetStationAnalyticsAsync(startDate, endDate);
        return Ok(analytics);
    }

    /// <summary>
    /// Get analytics for a specific station
    /// </summary>
    [HttpGet("{stationId}")]
    [ProducesResponseType(typeof(StationAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStationAnalytics(
        Guid stationId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetStationAnalyticsByIdAsync(stationId, startDate, endDate);
        
        if (analytics == null)
            return NotFound(new { error = $"No analytics found for station {stationId}" });

        return Ok(analytics);
    }
}