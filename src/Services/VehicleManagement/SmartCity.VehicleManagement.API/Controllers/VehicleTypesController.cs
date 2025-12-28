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
public class VehicleTypesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleTypesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get all vehicle types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllVehicleTypes()
    {
        var types = await _vehicleService.GetAllVehicleTypesAsync();
        return Ok(types);
    }
}