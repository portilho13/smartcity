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

using Microsoft.AspNetCore.Mvc;
using SmartCity.VehicleManagement.Core.DTOs;
using SmartCity.VehicleManagement.Core.Services;

namespace SmartCity.VehicleManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class StationsController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public StationsController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get all stations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStations()
    {
        var stations = await _vehicleService.GetAllStationsAsync();
        return Ok(stations);
    }

    /// <summary>
    /// Get nearby stations
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<StationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearbyStations(
        [FromQuery] decimal lat, 
        [FromQuery] decimal lon, 
        [FromQuery] int radius = 2)
    {
        var stations = await _vehicleService.GetNearbyStationsAsync(lat, lon, radius);
        return Ok(stations);
    }

    /// <summary>
    /// Get station by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStationById(Guid id)
    {
        try
        {
            var station = await _vehicleService.GetStationByIdAsync(id);
            return Ok(station);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Station with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get station availability
    /// </summary>
    [HttpGet("{id}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStationAvailability(Guid id)
    {
        try
        {
            var station = await _vehicleService.GetStationByIdAsync(id);
            return Ok(new
            {
                stationId = station.Id,
                stationName = station.Name,
                totalCapacity = station.TotalCapacity,
                availableSlots = station.AvailableSlots,
                occupiedSlots = station.OccupiedSlots,
                hasCharging = station.HasCharging,
                status = station.Status
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Station with ID {id} not found" });
        }
    }
}