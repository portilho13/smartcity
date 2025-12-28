/*
 * ===================================================================================
 * TRABALHO PRÁTICO: Integração de Sistemas de Informação (ISI)
 * -----------------------------------------------------------------------------------
 * Nome: Mario Junior Manhente Portilho
 * Número: a27989
 * Curso: Engenharia de Sistemas Informáticos
 * Ano Letivo: 2025/2026
 * ===================================================================================
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.AnalyticsService.Core.DTOs;
using SmartCity.AnalyticsService.Core.Services;

namespace SmartCity.AnalyticsService.API.Controllers;

[ApiController]
[Route("api/v1/analytics/[controller]")]
[Authorize(Roles = "admin")]
public class SystemController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<SystemController> _logger;

    public SystemController(IAnalyticsService analyticsService, ILogger<SystemController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get overall system analytics
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SystemAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetSystemAnalyticsAsync(startDate, endDate);
        return Ok(analytics);
    }

    /// <summary>
    /// Get revenue by date
    /// </summary>
    [HttpGet("revenue/daily")]
    [ProducesResponseType(typeof(IEnumerable<RevenueAnalyticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRevenueByDate(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetRevenueByDateAsync(startDate, endDate);
        return Ok(analytics);
    }

    /// <summary>
    /// Get total revenue
    /// </summary>
    [HttpGet("revenue/total")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalRevenue(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var revenue = await _analyticsService.GetTotalRevenueAsync(startDate, endDate);
        return Ok(new { totalRevenue = revenue });
    }

    /// <summary>
    /// Get popular routes
    /// </summary>
    [HttpGet("routes/popular")]
    [ProducesResponseType(typeof(IEnumerable<PopularRouteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularRoutes(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int limit = 10)
    {
        var routes = await _analyticsService.GetPopularRoutesAsync(startDate, endDate, limit);
        return Ok(routes);
    }

    /// <summary>
    /// Get vehicle type analytics
    /// </summary>
    [HttpGet("vehicle-types")]
    [ProducesResponseType(typeof(IEnumerable<VehicleTypeAnalyticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicleTypeAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetVehicleTypeAnalyticsAsync(startDate, endDate);
        return Ok(analytics);
    }

    /// <summary>
    /// Get hourly usage patterns
    /// </summary>
    [HttpGet("usage/hourly")]
    [ProducesResponseType(typeof(IEnumerable<HourlyUsageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHourlyUsage(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var usage = await _analyticsService.GetHourlyUsageAsync(startDate, endDate);
        return Ok(usage);
    }
}