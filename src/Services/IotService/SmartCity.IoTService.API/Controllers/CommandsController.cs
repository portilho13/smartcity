using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.IoTService.Core.DTOs;
using SmartCity.IoTService.Core.Services;

namespace SmartCity.IoTService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CommandsController : ControllerBase
{
    private readonly IIoTService _iotService;
    private readonly ILogger<CommandsController> _logger;

    public CommandsController(IIoTService iotService, ILogger<CommandsController> logger)
    {
        _iotService = iotService;
        _logger = logger;
    }

    /// <summary>
    /// Get command by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IoTCommandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommandById(Guid id)
    {
        var command = await _iotService.GetCommandByIdAsync(id);
        
        if (command == null)
            return NotFound(new { error = $"Command with ID {id} not found" });

        return Ok(command);
    }

    /// <summary>
    /// Get commands for a vehicle
    /// </summary>
    [HttpGet("vehicles/{vehicleId}")]
    [ProducesResponseType(typeof(IEnumerable<IoTCommandDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommandsByVehicle(
        Guid vehicleId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var commands = await _iotService.GetCommandsByVehicleAsync(vehicleId, page, pageSize);
        return Ok(commands);
    }

    /// <summary>
    /// Get pending commands for a device
    /// </summary>
    [HttpGet("devices/{deviceId}/pending")]
    [AllowAnonymous] // IoT devices might use API keys
    [ProducesResponseType(typeof(IEnumerable<IoTCommandDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingCommands(Guid deviceId)
    {
        var commands = await _iotService.GetPendingCommandsAsync(deviceId);
        return Ok(commands);
    }

    /// <summary>
    /// Send a command to a vehicle
    /// </summary>
    [HttpPost("vehicles/{vehicleId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IoTCommandDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> SendCommand(Guid vehicleId, [FromBody] SendCommandRequest request)
    {
        try
        {
            var command = await _iotService.SendCommandAsync(vehicleId, request);
            return CreatedAtAction(nameof(GetCommandById), new { id = command.Id }, command);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update command status (IoT devices endpoint)
    /// </summary>
    [HttpPut("{id}/status")]
    [AllowAnonymous] // IoT devices might use API keys
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCommandStatus(
        Guid id, 
        [FromQuery] string status,
        [FromBody] UpdateCommandStatusRequest? request = null)
    {
        var success = await _iotService.UpdateCommandStatusAsync(
            id, 
            status, 
            request?.Response, 
            request?.ErrorMessage
        );
        
        if (!success)
            return NotFound(new { error = $"Command with ID {id} not found" });

        return Ok(new { message = "Command status updated successfully" });
    }
}

public class UpdateCommandStatusRequest
{
    public string? Response { get; set; }
    public string? ErrorMessage { get; set; }
}   