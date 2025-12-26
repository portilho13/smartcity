using CoreWCF;
using SmartCity.DataLayer.IoTService.Contracts;
using SmartCity.DataLayer.IoTService.DataContracts;
using SmartCity.DataLayer.IoTService.Repositories;

namespace SmartCity.DataLayer.IoTService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class IoTDataService : IIoTDataService
{
    private readonly IoTRepository _repository;
    private readonly ILogger<IoTDataService> _logger;

    public IoTDataService(IoTRepository repository, ILogger<IoTDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IoTDeviceDataContract?> GetDeviceByIdAsync(Guid deviceId)
    {
        _logger.LogInformation("SOAP: GetDeviceById {DeviceId}", deviceId);
        return await _repository.GetDeviceByIdAsync(deviceId);
    }

    public async Task<IoTDeviceDataContract?> GetDeviceByVehicleIdAsync(Guid vehicleId)
    {
        _logger.LogInformation("SOAP: GetDeviceByVehicleId {VehicleId}", vehicleId);
        return await _repository.GetDeviceByVehicleIdAsync(vehicleId);
    }

    public async Task<IoTDeviceDataContract?> GetDeviceByIdentifierAsync(string deviceIdentifier)
    {
        _logger.LogInformation("SOAP: GetDeviceByIdentifier {DeviceIdentifier}", deviceIdentifier);
        return await _repository.GetDeviceByIdentifierAsync(deviceIdentifier);
    }

    public async Task<IoTDeviceDataContract[]> GetAllDevicesAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAllDevices Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<IoTDeviceDataContract> CreateDeviceAsync(CreateIoTDeviceRequest request)
    {
        _logger.LogInformation("SOAP: CreateDevice Vehicle={VehicleId} Identifier={Identifier}", 
            request.VehicleId, request.DeviceIdentifier);
        return await _repository.CreateAsync(request);
    }

    public async Task<bool> UpdateDeviceStatusAsync(Guid deviceId, string status)
    {
        _logger.LogInformation("SOAP: UpdateDeviceStatus {DeviceId} -> {Status}", deviceId, status);
        return await _repository.UpdateStatusAsync(deviceId, status);
    }

    public async Task<bool> UpdateDeviceFirmwareAsync(Guid deviceId, string firmwareVersion)
    {
        _logger.LogInformation("SOAP: UpdateDeviceFirmware {DeviceId} -> {Version}", deviceId, firmwareVersion);
        return await _repository.UpdateFirmwareAsync(deviceId, firmwareVersion);
    }

    public async Task<IoTTelemetryDataContract?> GetLatestTelemetryAsync(Guid vehicleId)
    {
        _logger.LogInformation("SOAP: GetLatestTelemetry {VehicleId}", vehicleId);
        return await _repository.GetLatestTelemetryAsync(vehicleId);
    }

    public async Task<IoTTelemetryDataContract[]> GetTelemetryByVehicleAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetTelemetryByVehicle {VehicleId} From={Start} To={End}", 
            vehicleId, startDate, endDate);
        return await _repository.GetTelemetryByVehicleAsync(vehicleId, startDate, endDate);
    }

    public async Task<IoTTelemetryDataContract[]> GetTelemetryByDeviceAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetTelemetryByDevice {DeviceId} From={Start} To={End}", 
            deviceId, startDate, endDate);
        return await _repository.GetTelemetryByDeviceAsync(deviceId, startDate, endDate);
    }

    public async Task<IoTTelemetryDataContract> CreateTelemetryAsync(CreateTelemetryRequest request)
    {
        _logger.LogInformation("SOAP: CreateTelemetry Device={DeviceId} Vehicle={VehicleId}", 
            request.DeviceId, request.VehicleId);
        return await _repository.CreateTelemetryAsync(request);
    }

    public async Task<IoTCommandDataContract?> GetCommandByIdAsync(Guid commandId)
    {
        _logger.LogInformation("SOAP: GetCommandById {CommandId}", commandId);
        return await _repository.GetCommandByIdAsync(commandId);
    }

    public async Task<IoTCommandDataContract[]> GetCommandsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetCommandsByVehicle {VehicleId} Page={Page} Size={Size}", 
            vehicleId, pageNumber, pageSize);
        return await _repository.GetCommandsByVehicleAsync(vehicleId, pageNumber, pageSize);
    }

    public async Task<IoTCommandDataContract[]> GetPendingCommandsAsync(Guid deviceId)
    {
        _logger.LogInformation("SOAP: GetPendingCommands {DeviceId}", deviceId);
        return await _repository.GetPendingCommandsAsync(deviceId);
    }

    public async Task<IoTCommandDataContract> SendCommandAsync(SendCommandRequest request)
    {
        _logger.LogInformation("SOAP: SendCommand Device={DeviceId} Type={CommandType}", 
            request.DeviceId, request.CommandType);
        return await _repository.SendCommandAsync(request);
    }

    public async Task<bool> UpdateCommandStatusAsync(Guid commandId, string status, string? response, string? errorMessage)
    {
        _logger.LogInformation("SOAP: UpdateCommandStatus {CommandId} -> {Status}", commandId, status);
        return await _repository.UpdateCommandStatusAsync(commandId, status, response, errorMessage);
    }

    public async Task<IoTAlertDataContract?> GetAlertByIdAsync(Guid alertId)
    {
        _logger.LogInformation("SOAP: GetAlertById {AlertId}", alertId);
        return await _repository.GetAlertByIdAsync(alertId);
    }

    public async Task<IoTAlertDataContract[]> GetAlertsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAlertsByVehicle {VehicleId} Page={Page} Size={Size}", 
            vehicleId, pageNumber, pageSize);
        return await _repository.GetAlertsByVehicleAsync(vehicleId, pageNumber, pageSize);
    }

    public async Task<IoTAlertDataContract[]> GetActiveAlertsAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetActiveAlerts Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetActiveAlertsAsync(pageNumber, pageSize);
    }

    public async Task<IoTAlertDataContract> CreateAlertAsync(CreateAlertRequest request)
    {
        _logger.LogInformation("SOAP: CreateAlert Vehicle={VehicleId} Type={AlertType}", 
            request.VehicleId, request.AlertType);
        return await _repository.CreateAlertAsync(request);
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId, string resolvedBy)
    {
        _logger.LogInformation("SOAP: ResolveAlert {AlertId} By={ResolvedBy}", alertId, resolvedBy);
        return await _repository.ResolveAlertAsync(alertId, resolvedBy);
    }
}