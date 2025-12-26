using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.AnalyticsService.Core.DTOs;
using SmartCity.AnalyticsService.Core.Services;

namespace SmartCity.AnalyticsService.API.Controllers;

[ApiController]
[Route("api/v1/analytics/[controller]")]
[Authorize(Roles = "admin")]
public class VehiclesController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IAnalyticsService analyticsService, ILogger<VehiclesController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get analytics for a specific vehicle
    /// </summary>
    [HttpGet("{vehicleId}")]
    [ProducesResponseType(typeof(VehicleAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleAnalytics(
        Guid vehicleId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetVehicleAnalyticsAsync(vehicleId, startDate, endDate);
        
        if (analytics == null)
            return NotFound(new { error = $"No analytics found for vehicle {vehicleId}" });

        return Ok(analytics);
    }

    /// <summary>
    /// Get top performing vehicles
    /// </summary>
    [HttpGet("top")]
    [ProducesResponseType(typeof(IEnumerable<VehicleAnalyticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopPerformingVehicles(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int limit = 10)
    {
        var analytics = await _analyticsService.GetTopPerformingVehiclesAsync(startDate, endDate, limit);
        return Ok(analytics);
    }
}