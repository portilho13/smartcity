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
using SmartCity.VehicleManagement.Core.DTOs;
using SmartCity.VehicleManagement.Core.Services;

namespace SmartCity.VehicleManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all vehicles (paginated)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVehicles([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var vehicles = await _vehicleService.GetAllVehiclesAsync(page, pageSize);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get available vehicles
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableVehicles()
    {
        var vehicles = await _vehicleService.GetAvailableVehiclesAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Get nearby vehicles
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearbyVehicles(
        [FromQuery] decimal lat, 
        [FromQuery] decimal lon, 
        [FromQuery] int radius = 2)
    {
        var vehicles = await _vehicleService.GetNearbyVehiclesAsync(lat, lon, radius);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get vehicle by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleById(Guid id)
    {
        try
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            return Ok(vehicle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Vehicle with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get vehicle by QR code
    /// </summary>
    [HttpGet("qr/{qrCode}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVehicleByQrCode(string qrCode)
    {
        var vehicle = await _vehicleService.GetVehicleByQrCodeAsync(qrCode);
        if (vehicle == null)
            return NotFound(new { error = $"Vehicle with QR code {qrCode} not found" });

        return Ok(vehicle);
    }

    /// <summary>
    /// Update vehicle location (IoT endpoint)
    /// </summary>
    [HttpPut("{id}/location")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVehicleLocation(Guid id, [FromBody] UpdateVehicleLocationRequest request)
    {
        var success = await _vehicleService.UpdateVehicleLocationAsync(id, request);
        if (!success)
            return NotFound(new { error = $"Vehicle with ID {id} not found" });

        return Ok(new { message = "Location updated successfully" });
    }

    /// <summary>
    /// Update vehicle status
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVehicleStatus(Guid id, [FromBody] UpdateVehicleStatusRequest request)
    {
        var success = await _vehicleService.UpdateVehicleStatusAsync(id, request.Status);
        if (!success)
            return NotFound(new { error = $"Vehicle with ID {id} not found" });

        return Ok(new { message = "Status updated successfully" });
    }
}