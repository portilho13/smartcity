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

using System.ServiceModel;
using SmartCity.DataLayer.VehicleService.DataContracts;

namespace SmartCity.DataLayer.VehicleService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/vehicledata/v1")]
public interface IVehicleDataService
{
    [OperationContract]
    Task<VehicleDataContract?> GetVehicleByIdAsync(Guid vehicleId);

    [OperationContract]
    Task<VehicleDataContract?> GetVehicleByQrCodeAsync(string qrCode);

    [OperationContract]
    Task<VehicleDataContract[]> GetAllVehiclesAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<VehicleDataContract[]> GetAvailableVehiclesAsync();

    [OperationContract]
    Task<VehicleDataContract[]> GetNearbyVehiclesAsync(decimal latitude, decimal longitude, int radiusKm);

    [OperationContract]
    Task<bool> UpdateVehicleLocationAsync(Guid vehicleId, decimal latitude, decimal longitude, int? batteryLevel);

    [OperationContract]
    Task<bool> UpdateVehicleStatusAsync(Guid vehicleId, string status);

    [OperationContract]
    Task<StationDataContract?> GetStationByIdAsync(Guid stationId);

    [OperationContract]
    Task<StationDataContract[]> GetAllStationsAsync();

    [OperationContract]
    Task<StationDataContract[]> GetNearbyStationsAsync(decimal latitude, decimal longitude, int radiusKm);

    [OperationContract]
    Task<VehicleTypeDataContract[]> GetAllVehicleTypesAsync();
}