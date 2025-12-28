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
using SmartCity.IoTService.Core.DTOs;
using SmartCity.IoTService.Core.Services;

namespace SmartCity.IoTService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TelemetryController : ControllerBase
{
    private readonly IIoTService _iotService;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(IIoTService iotService, ILogger<TelemetryController> logger)
    {
        _iotService = iotService;
        _logger = logger;
    }

    /// <summary>
    /// Get latest telemetry for a vehicle
    /// </summary>
    [HttpGet("vehicles/{vehicleId}/latest")]
    [ProducesResponseType(typeof(IoTTelemetryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestTelemetry(Guid vehicleId)
    {
        var telemetry = await _iotService.GetLatestTelemetryAsync(vehicleId);
        
        if (telemetry == null)
            return NotFound(new { error = $"No telemetry found for vehicle {vehicleId}" });

        return Ok(telemetry);
    }

    /// <summary>
    /// Get telemetry history for a vehicle
    /// </summary>
    [HttpGet("vehicles/{vehicleId}")]
    [ProducesResponseType(typeof(IEnumerable<IoTTelemetryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTelemetryByVehicle(
        Guid vehicleId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddHours(-24);
        var end = endDate ?? DateTime.UtcNow;

        var telemetry = await _iotService.GetTelemetryByVehicleAsync(vehicleId, start, end);
        return Ok(telemetry);
    }

    /// <summary>
    /// Get telemetry history for a device
    /// </summary>
    [HttpGet("devices/{deviceId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<IoTTelemetryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTelemetryByDevice(
        Guid deviceId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddHours(-24);
        var end = endDate ?? DateTime.UtcNow;

        var telemetry = await _iotService.GetTelemetryByDeviceAsync(deviceId, start, end);
        return Ok(telemetry);
    }

    /// <summary>
    /// Create telemetry data (IoT devices endpoint)
    /// </summary>
    [HttpPost]
    [AllowAnonymous] // IoT devices might use API keys instead
    [ProducesResponseType(typeof(IoTTelemetryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTelemetry([FromBody] CreateTelemetryRequest request)
    {
        var telemetry = await _iotService.CreateTelemetryAsync(request);
        return CreatedAtAction(
            nameof(GetLatestTelemetry), 
            new { vehicleId = telemetry.VehicleId }, 
            telemetry
        );
    }
}