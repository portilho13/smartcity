using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.IoTService.Core.DTOs;
using SmartCity.IoTService.Core.Services;

namespace SmartCity.IoTService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IIoTService _iotService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IIoTService iotService, ILogger<AlertsController> logger)
    {
        _iotService = iotService;
        _logger = logger;
    }

    /// <summary>
    /// Get all active alerts (Admin only)
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<IoTAlertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var alerts = await _iotService.GetActiveAlertsAsync(page, pageSize);
        return Ok(alerts);
    }

    /// <summary>
    /// Get alert by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IoTAlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlertById(Guid id)
    {
        var alert = await _iotService.GetAlertByIdAsync(id);
        
        if (alert == null)
            return NotFound(new { error = $"Alert with ID {id} not found" });

        return Ok(alert);
    }

    /// <summary>
    /// Get alerts for a vehicle
    /// </summary>
    [HttpGet("vehicles/{vehicleId}")]
    [ProducesResponseType(typeof(IEnumerable<IoTAlertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertsByVehicle(
        Guid vehicleId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var alerts = await _iotService.GetAlertsByVehicleAsync(vehicleId, page, pageSize);
        return Ok(alerts);
    }

    /// <summary>
    /// Create an alert (IoT devices or system endpoint)
    /// </summary>
    [HttpPost("vehicles/{vehicleId}")]
    [AllowAnonymous] // IoT devices might use API keys
    [ProducesResponseType(typeof(IoTAlertDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAlert(Guid vehicleId, [FromBody] CreateAlertRequest request)
    {
        try
        {
            var alert = await _iotService.CreateAlertAsync(vehicleId, request);
            return CreatedAtAction(nameof(GetAlertById), new { id = alert.Id }, alert);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Resolve an alert
    /// </summary>
    [HttpPost("{id}/resolve")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(Guid id, [FromBody] ResolveAlertRequest request)
    {
        var success = await _iotService.ResolveAlertAsync(id, request.ResolvedBy);
        
        if (!success)
            return NotFound(new { error = $"Alert with ID {id} not found or already resolved" });

        return Ok(new { message = "Alert resolved successfully" });
    }
}