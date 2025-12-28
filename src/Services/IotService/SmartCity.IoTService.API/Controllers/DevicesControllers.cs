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
public class DevicesController : ControllerBase
{
    private readonly IIoTService _iotService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IIoTService iotService, ILogger<DevicesController> logger)
    {
        _iotService = iotService;
        _logger = logger;
    }

    /// <summary>
    /// Get all IoT devices (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<IoTDeviceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDevices([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var devices = await _iotService.GetAllDevicesAsync(page, pageSize);
        return Ok(devices);
    }

    /// <summary>
    /// Get device by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IoTDeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceById(Guid id)
    {
        var device = await _iotService.GetDeviceByIdAsync(id);
        
        if (device == null)
            return NotFound(new { error = $"Device with ID {id} not found" });

        return Ok(device);
    }

    /// <summary>
    /// Get device by vehicle ID
    /// </summary>
    [HttpGet("vehicles/{vehicleId}")]
    [ProducesResponseType(typeof(IoTDeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceByVehicleId(Guid vehicleId)
    {
        var device = await _iotService.GetDeviceByVehicleIdAsync(vehicleId);
        
        if (device == null)
            return NotFound(new { error = $"No device found for vehicle {vehicleId}" });

        return Ok(device);
    }

    /// <summary>
    /// Get device by identifier
    /// </summary>
    [HttpGet("identifier/{identifier}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IoTDeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceByIdentifier(string identifier)
    {
        var device = await _iotService.GetDeviceByIdentifierAsync(identifier);
        
        if (device == null)
            return NotFound(new { error = $"Device with identifier {identifier} not found" });

        return Ok(device);
    }

    /// <summary>
    /// Create a new IoT device
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IoTDeviceDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
    {
        var device = await _iotService.CreateDeviceAsync(request);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    /// <summary>
    /// Update device status
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDeviceStatus(Guid id, [FromBody] UpdateDeviceStatusRequest request)
    {
        var success = await _iotService.UpdateDeviceStatusAsync(id, request.Status);
        
        if (!success)
            return NotFound(new { error = $"Device with ID {id} not found" });

        return Ok(new { message = "Device status updated successfully" });
    }

    /// <summary>
    /// Update device firmware
    /// </summary>
    [HttpPut("{id}/firmware")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDeviceFirmware(Guid id, [FromBody] UpdateFirmwareRequest request)
    {
        var success = await _iotService.UpdateDeviceFirmwareAsync(id, request.FirmwareVersion);
        
        if (!success)
            return NotFound(new { error = $"Device with ID {id} not found" });

        return Ok(new { message = "Device firmware updated successfully" });
    }
}