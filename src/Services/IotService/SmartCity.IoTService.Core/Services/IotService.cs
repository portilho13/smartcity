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

using SmartCity.IoTService.Core.DTOs;
using SmartCity.DataLayer.IoTService.Contracts;
using SmartCity.DataLayer.IoTService.DataContracts;
using Microsoft.Extensions.Logging;
using CreateAlertRequest = SmartCity.IoTService.Core.DTOs.CreateAlertRequest;
using CreateTelemetryRequest = SmartCity.IoTService.Core.DTOs.CreateTelemetryRequest;
using SendCommandRequest = SmartCity.IoTService.Core.DTOs.SendCommandRequest;

namespace SmartCity.IoTService.Core.Services;

public interface IIoTService
{
    Task<IoTDeviceDto?> GetDeviceByIdAsync(Guid deviceId);
    Task<IoTDeviceDto?> GetDeviceByVehicleIdAsync(Guid vehicleId);
    Task<IoTDeviceDto?> GetDeviceByIdentifierAsync(string deviceIdentifier);
    Task<IEnumerable<IoTDeviceDto>> GetAllDevicesAsync(int page, int pageSize);
    Task<IoTDeviceDto> CreateDeviceAsync(CreateDeviceRequest request);
    Task<bool> UpdateDeviceStatusAsync(Guid deviceId, string status);
    Task<bool> UpdateDeviceFirmwareAsync(Guid deviceId, string firmwareVersion);
    
    Task<IoTTelemetryDto?> GetLatestTelemetryAsync(Guid vehicleId);
    Task<IEnumerable<IoTTelemetryDto>> GetTelemetryByVehicleAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<IoTTelemetryDto>> GetTelemetryByDeviceAsync(Guid deviceId, DateTime startDate, DateTime endDate);
    Task<IoTTelemetryDto> CreateTelemetryAsync(CreateTelemetryRequest request);
    
    Task<IoTCommandDto?> GetCommandByIdAsync(Guid commandId);
    Task<IEnumerable<IoTCommandDto>> GetCommandsByVehicleAsync(Guid vehicleId, int page, int pageSize);
    Task<IEnumerable<IoTCommandDto>> GetPendingCommandsAsync(Guid deviceId);
    Task<IoTCommandDto> SendCommandAsync(Guid vehicleId, SendCommandRequest request);
    Task<bool> UpdateCommandStatusAsync(Guid commandId, string status, string? response, string? errorMessage);
    
    Task<IoTAlertDto?> GetAlertByIdAsync(Guid alertId);
    Task<IEnumerable<IoTAlertDto>> GetAlertsByVehicleAsync(Guid vehicleId, int page, int pageSize);
    Task<IEnumerable<IoTAlertDto>> GetActiveAlertsAsync(int page, int pageSize);
    Task<IoTAlertDto> CreateAlertAsync(Guid vehicleId, CreateAlertRequest request);
    Task<bool> ResolveAlertAsync(Guid alertId, string resolvedBy);
}

public class IoTService : IIoTService
{
    private readonly IIoTDataService _soapClient;
    private readonly ILogger<IoTService> _logger;

    public IoTService(IIoTDataService soapClient, ILogger<IoTService> logger)
    {
        _soapClient = soapClient;
        _logger = logger;
    }

    public async Task<IoTDeviceDto?> GetDeviceByIdAsync(Guid deviceId)
    {
        var deviceData = await _soapClient.GetDeviceByIdAsync(deviceId);
        return deviceData != null ? MapToDto(deviceData) : null;
    }

    public async Task<IoTDeviceDto?> GetDeviceByVehicleIdAsync(Guid vehicleId)
    {
        var deviceData = await _soapClient.GetDeviceByVehicleIdAsync(vehicleId);
        return deviceData != null ? MapToDto(deviceData) : null;
    }

    public async Task<IoTDeviceDto?> GetDeviceByIdentifierAsync(string deviceIdentifier)
    {
        var deviceData = await _soapClient.GetDeviceByIdentifierAsync(deviceIdentifier);
        return deviceData != null ? MapToDto(deviceData) : null;
    }

    public async Task<IEnumerable<IoTDeviceDto>> GetAllDevicesAsync(int page, int pageSize)
    {
        var devicesData = await _soapClient.GetAllDevicesAsync(page, pageSize);
        return devicesData.Select(MapToDto);
    }

    public async Task<IoTDeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
    {
        var createRequest = new SmartCity.DataLayer.IoTService.DataContracts.CreateIoTDeviceRequest
        {
            VehicleId = request.VehicleId,
            DeviceIdentifier = request.DeviceIdentifier,
            DeviceType = request.DeviceType,
            FirmwareVersion = request.FirmwareVersion
        };

        var deviceData = await _soapClient.CreateDeviceAsync(createRequest);
        return MapToDto(deviceData);
    }

    public async Task<bool> UpdateDeviceStatusAsync(Guid deviceId, string status)
    {
        return await _soapClient.UpdateDeviceStatusAsync(deviceId, status);
    }

    public async Task<bool> UpdateDeviceFirmwareAsync(Guid deviceId, string firmwareVersion)
    {
        return await _soapClient.UpdateDeviceFirmwareAsync(deviceId, firmwareVersion);
    }

    public async Task<IoTTelemetryDto?> GetLatestTelemetryAsync(Guid vehicleId)
    {
        var telemetryData = await _soapClient.GetLatestTelemetryAsync(vehicleId);
        return telemetryData != null ? MapToDto(telemetryData) : null;
    }

    public async Task<IEnumerable<IoTTelemetryDto>> GetTelemetryByVehicleAsync(Guid vehicleId, DateTime startDate,
        DateTime endDate)
    {
        var telemetryData = await _soapClient.GetTelemetryByVehicleAsync(vehicleId, startDate, endDate);
        return telemetryData.Select(MapToDto);
    }

    public async Task<IEnumerable<IoTTelemetryDto>> GetTelemetryByDeviceAsync(Guid deviceId, DateTime startDate,
        DateTime endDate)
    {
        var telemetryData = await _soapClient.GetTelemetryByDeviceAsync(deviceId, startDate, endDate);
        return telemetryData.Select(MapToDto);
    }

    public async Task<IoTTelemetryDto> CreateTelemetryAsync(CreateTelemetryRequest request)
    {
        var createRequest = new SmartCity.DataLayer.IoTService.DataContracts.CreateTelemetryRequest
        {
            DeviceId = request.DeviceId,
            VehicleId = request.VehicleId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Speed = request.Speed,
            BatteryLevel = request.BatteryLevel,
            BatteryVoltage = request.BatteryVoltage,
            Temperature = request.Temperature,
            IsLocked = request.IsLocked,
            IsCharging = request.IsCharging,
            Odometer = request.Odometer,
            ErrorCodes = request.ErrorCodes,
            Timestamp = request.Timestamp
        };

        var telemetryData = await _soapClient.CreateTelemetryAsync(createRequest);
        return MapToDto(telemetryData);
    }

    public async Task<IoTCommandDto?> GetCommandByIdAsync(Guid commandId)
    {
        var commandData = await _soapClient.GetCommandByIdAsync(commandId);
        return commandData != null ? MapToDto(commandData) : null;
    }

    public async Task<IEnumerable<IoTCommandDto>> GetCommandsByVehicleAsync(Guid vehicleId, int page, int pageSize)
    {
        var commandsData = await _soapClient.GetCommandsByVehicleAsync(vehicleId, page, pageSize);
        return commandsData.Select(MapToDto);
    }

    public async Task<IEnumerable<IoTCommandDto>> GetPendingCommandsAsync(Guid deviceId)
    {
        var commandsData = await _soapClient.GetPendingCommandsAsync(deviceId);
        return commandsData.Select(MapToDto);
    }

    public async Task<IoTCommandDto> SendCommandAsync(Guid vehicleId, SendCommandRequest request)
    {
        // First, get the device for this vehicle
        var device = await _soapClient.GetDeviceByVehicleIdAsync(vehicleId);
        if (device == null)
            throw new InvalidOperationException($"No device found for vehicle {vehicleId}");

        var sendRequest = new SmartCity.DataLayer.IoTService.DataContracts.SendCommandRequest
        {
            DeviceId = device.Id,
            VehicleId = vehicleId,
            CommandType = request.CommandType,
            Parameters = request.Parameters
        };

        var commandData = await _soapClient.SendCommandAsync(sendRequest);
        return MapToDto(commandData);
    }

    public async Task<bool> UpdateCommandStatusAsync(Guid commandId, string status, string? response,
        string? errorMessage)
    {
        return await _soapClient.UpdateCommandStatusAsync(commandId, status, response, errorMessage);
    }

    public async Task<IoTAlertDto?> GetAlertByIdAsync(Guid alertId)
    {
        var alertData = await _soapClient.GetAlertByIdAsync(alertId);
        return alertData != null ? MapToDto(alertData) : null;
    }

    public async Task<IEnumerable<IoTAlertDto>> GetAlertsByVehicleAsync(Guid vehicleId, int page, int pageSize)
    {
        var alertsData = await _soapClient.GetAlertsByVehicleAsync(vehicleId, page, pageSize);
        return alertsData.Select(MapToDto);
    }

    public async Task<IEnumerable<IoTAlertDto>> GetActiveAlertsAsync(int page, int pageSize)
    {
        var alertsData = await _soapClient.GetActiveAlertsAsync(page, pageSize);
        return alertsData.Select(MapToDto);
    }

    public async Task<IoTAlertDto> CreateAlertAsync(Guid vehicleId, CreateAlertRequest request)
    {
        // First, get the device for this vehicle
        var device = await _soapClient.GetDeviceByVehicleIdAsync(vehicleId);
        if (device == null)
            throw new InvalidOperationException($"No device found for vehicle {vehicleId}");

        var createRequest = new SmartCity.DataLayer.IoTService.DataContracts.CreateAlertRequest
        {
            DeviceId = device.Id,
            VehicleId = vehicleId,
            AlertType = request.AlertType,
            Severity = request.Severity,
            Message = request.Message,
            Details = request.Details
        };

        var alertData = await _soapClient.CreateAlertAsync(createRequest);
        return MapToDto(alertData);
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId, string resolvedBy)
    {
        return await _soapClient.ResolveAlertAsync(alertId, resolvedBy);
    }

    // Mapping methods
    private static IoTDeviceDto MapToDto(IoTDeviceDataContract data)
    {
        return new IoTDeviceDto
        {
            Id = data.Id,
            VehicleId = data.VehicleId,
            DeviceIdentifier = data.DeviceIdentifier,
            DeviceType = data.DeviceType,
            Status = data.Status,
            FirmwareVersion = data.FirmwareVersion,
            LastCommunication = data.LastCommunication,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt
        };
    }

    private static IoTTelemetryDto MapToDto(IoTTelemetryDataContract data)
    {
        return new IoTTelemetryDto
        {
            Id = data.Id,
            DeviceId = data.DeviceId,
            VehicleId = data.VehicleId,
            Latitude = data.Latitude,
            Longitude = data.Longitude,
            Speed = data.Speed,
            BatteryLevel = data.BatteryLevel,
            BatteryVoltage = data.BatteryVoltage,
            Temperature = data.Temperature,
            IsLocked = data.IsLocked,
            IsCharging = data.IsCharging,
            Odometer = data.Odometer,
            ErrorCodes = data.ErrorCodes,
            Timestamp = data.Timestamp, CreatedAt = data.CreatedAt
        };
    }

    private static IoTCommandDto MapToDto(IoTCommandDataContract data)
    {
        return new IoTCommandDto
        {
            Id = data.Id,
            DeviceId = data.DeviceId,
            VehicleId = data.VehicleId,
            CommandType = data.CommandType,
            Parameters = data.Parameters,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            SentAt = data.SentAt,
            AcknowledgedAt = data.AcknowledgedAt,
            CompletedAt = data.CompletedAt,
            Response = data.Response,
            ErrorMessage = data.ErrorMessage
        };
    }

    private static IoTAlertDto MapToDto(IoTAlertDataContract data)
    {
        return new IoTAlertDto
        {
            Id = data.Id,
            DeviceId = data.DeviceId,
            VehicleId = data.VehicleId,
            AlertType = data.AlertType,
            Severity = data.Severity,
            Message = data.Message,
            Details = data.Details,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            ResolvedAt = data.ResolvedAt,
            ResolvedBy = data.ResolvedBy
        };
    }
}