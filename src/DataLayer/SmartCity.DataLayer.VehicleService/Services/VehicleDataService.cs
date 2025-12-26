using CoreWCF;
using SmartCity.DataLayer.VehicleService.Contracts;
using SmartCity.DataLayer.VehicleService.DataContracts;
using SmartCity.DataLayer.VehicleService.Repositories;

namespace SmartCity.DataLayer.VehicleService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class VehicleDataService : IVehicleDataService
{
    private readonly VehicleRepository _repository;
    private readonly ILogger<VehicleDataService> _logger;

    public VehicleDataService(VehicleRepository repository, ILogger<VehicleDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<VehicleDataContract?> GetVehicleByIdAsync(Guid vehicleId)
    {
        _logger.LogInformation("SOAP: GetVehicleById {VehicleId}", vehicleId);
        return await _repository.GetByIdAsync(vehicleId);
    }

    public async Task<VehicleDataContract?> GetVehicleByQrCodeAsync(string qrCode)
    {
        _logger.LogInformation("SOAP: GetVehicleByQrCode {QrCode}", qrCode);
        return await _repository.GetByQrCodeAsync(qrCode);
    }

    public async Task<VehicleDataContract[]> GetAllVehiclesAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAllVehicles Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<VehicleDataContract[]> GetAvailableVehiclesAsync()
    {
        _logger.LogInformation("SOAP: GetAvailableVehicles");
        return await _repository.GetAvailableAsync();
    }

    public async Task<VehicleDataContract[]> GetNearbyVehiclesAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        _logger.LogInformation("SOAP: GetNearbyVehicles Lat={Lat} Lon={Lon} Radius={Radius}km", 
            latitude, longitude, radiusKm);
        return await _repository.GetNearbyAsync(latitude, longitude, radiusKm);
    }

    public async Task<bool> UpdateVehicleLocationAsync(Guid vehicleId, decimal latitude, decimal longitude, int? batteryLevel)
    {
        _logger.LogInformation("SOAP: UpdateVehicleLocation {VehicleId}", vehicleId);
        return await _repository.UpdateLocationAsync(vehicleId, latitude, longitude, batteryLevel);
    }

    public async Task<bool> UpdateVehicleStatusAsync(Guid vehicleId, string status)
    {
        _logger.LogInformation("SOAP: UpdateVehicleStatus {VehicleId} -> {Status}", vehicleId, status);
        return await _repository.UpdateStatusAsync(vehicleId, status);
    }

    public async Task<StationDataContract?> GetStationByIdAsync(Guid stationId)
    {
        _logger.LogInformation("SOAP: GetStationById {StationId}", stationId);
        return await _repository.GetStationByIdAsync(stationId);
    }

    public async Task<StationDataContract[]> GetAllStationsAsync()
    {
        _logger.LogInformation("SOAP: GetAllStations");
        return await _repository.GetAllStationsAsync();
    }

    public async Task<StationDataContract[]> GetNearbyStationsAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        _logger.LogInformation("SOAP: GetNearbyStations Lat={Lat} Lon={Lon} Radius={Radius}km", 
            latitude, longitude, radiusKm);
        return await _repository.GetNearbyStationsAsync(latitude, longitude, radiusKm);
    }

    public async Task<VehicleTypeDataContract[]> GetAllVehicleTypesAsync()
    {
        _logger.LogInformation("SOAP: GetAllVehicleTypes");
        return await _repository.GetAllVehicleTypesAsync();
    }
}