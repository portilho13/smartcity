using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.AnalyticsService.Core.DTOs;
using SmartCity.AnalyticsService.Core.Services;
using System.Security.Claims;

namespace SmartCity.AnalyticsService.API.Controllers;

[ApiController]
[Route("api/v1/analytics/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IAnalyticsService analyticsService, ILogger<UsersController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");
        
        return userId;
    }

    /// <summary>
    /// Get analytics for current user
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = GetCurrentUserId();
        var analytics = await _analyticsService.GetUserAnalyticsAsync(userId, startDate, endDate);
        
        if (analytics == null)
            return NotFound(new { error = "No analytics found" });

        return Ok(analytics);
    }

    /// <summary>
    /// Get analytics for a specific user (Admin only)
    /// </summary>
    [HttpGet("{userId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(UserAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAnalytics(
        Guid userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var analytics = await _analyticsService.GetUserAnalyticsAsync(userId, startDate, endDate);
        
        if (analytics == null)
            return NotFound(new { error = $"No analytics found for user {userId}" });

        return Ok(analytics);
    }

    /// <summary>
    /// Get top users by spending (Admin only)
    /// </summary>
    [HttpGet("top")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<UserAnalyticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopUsers(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int limit = 10)
    {
        var analytics = await _analyticsService.GetTopUsersAsync(startDate, endDate, limit);
        return Ok(analytics);
    }
}