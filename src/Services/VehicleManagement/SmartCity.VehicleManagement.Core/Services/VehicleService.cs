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

using SmartCity.VehicleManagement.Core.DTOs;
using SmartCity.DataLayer.VehicleService.Contracts;
using SmartCity.DataLayer.VehicleService.DataContracts;
using Microsoft.Extensions.Logging;

namespace SmartCity.VehicleManagement.Core.Services;

public interface IVehicleService
{
    Task<VehicleDto?> GetVehicleByIdAsync(Guid vehicleId);
    Task<VehicleDto?> GetVehicleByQrCodeAsync(string qrCode);
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync(int page, int pageSize);
    Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync();
    Task<IEnumerable<VehicleDto>> GetNearbyVehiclesAsync(decimal latitude, decimal longitude, int radiusKm);
    Task<bool> UpdateVehicleLocationAsync(Guid vehicleId, UpdateVehicleLocationRequest request);
    Task<bool> UpdateVehicleStatusAsync(Guid vehicleId, string status);
    
    Task<StationDto?> GetStationByIdAsync(Guid stationId);
    Task<IEnumerable<StationDto>> GetAllStationsAsync();
    Task<IEnumerable<StationDto>> GetNearbyStationsAsync(decimal latitude, decimal longitude, int radiusKm);
    
    Task<IEnumerable<VehicleTypeDto>> GetAllVehicleTypesAsync();
}

public class VehicleService : IVehicleService
{
    private readonly IVehicleDataService _soapClient;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(IVehicleDataService soapClient, ILogger<VehicleService> logger)
    {
        _soapClient = soapClient;
        _logger = logger;
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid vehicleId)
    {
        var vehicleData = await _soapClient.GetVehicleByIdAsync(vehicleId);
        if (vehicleData == null)
            throw new KeyNotFoundException($"Vehicle with ID {vehicleId} not found");

        return MapToDto(vehicleData);
    }

    public async Task<VehicleDto?> GetVehicleByQrCodeAsync(string qrCode)
    {
        var vehicleData = await _soapClient.GetVehicleByQrCodeAsync(qrCode);
        return vehicleData != null ? MapToDto(vehicleData) : null;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync(int page, int pageSize)
    {
        var vehiclesData = await _soapClient.GetAllVehiclesAsync(page, pageSize);
        return vehiclesData.Select(MapToDto);
    }

    public async Task<IEnumerable<VehicleDto>> GetAvailableVehiclesAsync()
    {
        var vehiclesData = await _soapClient.GetAvailableVehiclesAsync();
        return vehiclesData.Select(MapToDto);
    }

    public async Task<IEnumerable<VehicleDto>> GetNearbyVehiclesAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        var vehiclesData = await _soapClient.GetNearbyVehiclesAsync(latitude, longitude, radiusKm);
        return vehiclesData.Select(MapToDto);
    }

    public async Task<bool> UpdateVehicleLocationAsync(Guid vehicleId, UpdateVehicleLocationRequest request)
    {
        return await _soapClient.UpdateVehicleLocationAsync(vehicleId, request.Latitude, request.Longitude, request.BatteryLevel);
    }

    public async Task<bool> UpdateVehicleStatusAsync(Guid vehicleId, string status)
    {
        return await _soapClient.UpdateVehicleStatusAsync(vehicleId, status);
    }

    public async Task<StationDto?> GetStationByIdAsync(Guid stationId)
    {
        var stationData = await _soapClient.GetStationByIdAsync(stationId);
        if (stationData == null)
            throw new KeyNotFoundException($"Station with ID {stationId} not found");

        return MapToDto(stationData);
    }

    public async Task<IEnumerable<StationDto>> GetAllStationsAsync()
    {
        var stationsData = await _soapClient.GetAllStationsAsync();
        return stationsData.Select(MapToDto);
    }

    public async Task<IEnumerable<StationDto>> GetNearbyStationsAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        var stationsData = await _soapClient.GetNearbyStationsAsync(latitude, longitude, radiusKm);
        return stationsData.Select(MapToDto);
    }

    public async Task<IEnumerable<VehicleTypeDto>> GetAllVehicleTypesAsync()
    {
        var typesData = await _soapClient.GetAllVehicleTypesAsync();
        return typesData.Select(MapToDto);
    }

    // Mapping methods
    private static VehicleDto MapToDto(VehicleDataContract data)
    {
        return new VehicleDto
        {
            Id = data.Id,
            VehicleTypeId = data.VehicleTypeId,
            LicensePlate = data.LicensePlate,
            QrCode = data.QrCode,
            Model = data.Model,
            Manufacturer = data.Manufacturer,
            Year = data.Year,
            Color = data.Color,
            Status = data.Status,
            BatteryLevel = data.BatteryLevel,
            CurrentLatitude = data.CurrentLatitude,
            CurrentLongitude = data.CurrentLongitude,
            LastLocationUpdate = data.LastLocationUpdate,
            CurrentStationId = data.CurrentStationId,
            TotalDistanceKm = data.TotalDistanceKm,
            TotalTrips = data.TotalTrips,
            CreatedAt = data.CreatedAt
        };
    }

    private static StationDto MapToDto(StationDataContract data)
    {
        return new StationDto
        {
            Id = data.Id,
            Name = data.Name,
            StationType = data.StationType,
            Latitude = data.Latitude,
            Longitude = data.Longitude,
            Address = data.Address,
            City = data.City,
            TotalCapacity = data.TotalCapacity,
            AvailableSlots = data.AvailableSlots,
            Status = data.Status,
            HasCharging = data.HasCharging,
            CreatedAt = data.CreatedAt
        };
    }

    private static VehicleTypeDto MapToDto(VehicleTypeDataContract data)
    {
        return new VehicleTypeDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = data.Description,
            BasePricePerMinute = data.BasePricePerMinute,
            UnlockFee = data.UnlockFee,
            MaxSpeed = data.MaxSpeed,
            RequiresLicense = data.RequiresLicense,
            MinAge = data.MinAge
        };
    }
}