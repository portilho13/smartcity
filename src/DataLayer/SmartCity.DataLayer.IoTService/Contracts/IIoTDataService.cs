using System.ServiceModel;
using SmartCity.DataLayer.IoTService.DataContracts;

namespace SmartCity.DataLayer.IoTService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public interface IIoTDataService
{
    // Device operations
    [OperationContract]
    Task<IoTDeviceDataContract?> GetDeviceByIdAsync(Guid deviceId);

    [OperationContract]
    Task<IoTDeviceDataContract?> GetDeviceByVehicleIdAsync(Guid vehicleId);

    [OperationContract]
    Task<IoTDeviceDataContract?> GetDeviceByIdentifierAsync(string deviceIdentifier);

    [OperationContract]
    Task<IoTDeviceDataContract[]> GetAllDevicesAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<IoTDeviceDataContract> CreateDeviceAsync(CreateIoTDeviceRequest request);

    [OperationContract]
    Task<bool> UpdateDeviceStatusAsync(Guid deviceId, string status);

    [OperationContract]
    Task<bool> UpdateDeviceFirmwareAsync(Guid deviceId, string firmwareVersion);

    // Telemetry operations
    [OperationContract]
    Task<IoTTelemetryDataContract?> GetLatestTelemetryAsync(Guid vehicleId);

    [OperationContract]
    Task<IoTTelemetryDataContract[]> GetTelemetryByVehicleAsync(Guid vehicleId, DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<IoTTelemetryDataContract[]> GetTelemetryByDeviceAsync(Guid deviceId, DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<IoTTelemetryDataContract> CreateTelemetryAsync(CreateTelemetryRequest request);

    // Command operations
    [OperationContract]
    Task<IoTCommandDataContract?> GetCommandByIdAsync(Guid commandId);

    [OperationContract]
    Task<IoTCommandDataContract[]> GetCommandsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize);

    [OperationContract]
    Task<IoTCommandDataContract[]> GetPendingCommandsAsync(Guid deviceId);

    [OperationContract]
    Task<IoTCommandDataContract> SendCommandAsync(SendCommandRequest request);

    [OperationContract]
    Task<bool> UpdateCommandStatusAsync(Guid commandId, string status, string? response, string? errorMessage);

    // Alert operations
    [OperationContract]
    Task<IoTAlertDataContract?> GetAlertByIdAsync(Guid alertId);

    [OperationContract]
    Task<IoTAlertDataContract[]> GetAlertsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize);

    [OperationContract]
    Task<IoTAlertDataContract[]> GetActiveAlertsAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<IoTAlertDataContract> CreateAlertAsync(CreateAlertRequest request);

    [OperationContract]
    Task<bool> ResolveAlertAsync(Guid alertId, string resolvedBy);
}