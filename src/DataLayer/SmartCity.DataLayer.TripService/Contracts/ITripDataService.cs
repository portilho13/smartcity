using System.ServiceModel;
using SmartCity.DataLayer.TripService.DataContracts;

namespace SmartCity.DataLayer.TripService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public interface ITripDataService
{
    [OperationContract]
    Task<TripDataContract?> GetTripByIdAsync(Guid tripId);

    [OperationContract]
    Task<TripDataContract?> GetActiveTripByUserAsync(Guid userId);

    [OperationContract]
    Task<TripDataContract[]> GetTripsByUserAsync(Guid userId, int pageNumber, int pageSize);

    [OperationContract]
    Task<TripDataContract[]> GetTripsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize);

    [OperationContract]
    Task<TripDataContract[]> GetAllTripsAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<TripDataContract> CreateTripAsync(CreateTripRequest request);

    [OperationContract]
    Task<TripDataContract?> EndTripAsync(Guid tripId, EndTripRequest request);

    [OperationContract]
    Task<bool> CancelTripAsync(Guid tripId, string reason);

    [OperationContract]
    Task<bool> AddTripLocationAsync(Guid tripId, decimal latitude, decimal longitude, int? speed, int? batteryLevel);

    [OperationContract]
    Task<TripLocationDataContract[]> GetTripLocationsAsync(Guid tripId);

    [OperationContract]
    Task<bool> RateTripAsync(Guid tripId, int rating, string? review);

    [OperationContract]
    Task<TripStatisticsDataContract> GetUserStatisticsAsync(Guid userId, DateTime? startDate, DateTime? endDate);

    [OperationContract]
    Task<TripStatisticsDataContract> GetVehicleStatisticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate);

    [OperationContract]
    Task<TripStatisticsDataContract> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate);
}