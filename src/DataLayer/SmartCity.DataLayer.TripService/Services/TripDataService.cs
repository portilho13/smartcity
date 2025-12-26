using CoreWCF;
using SmartCity.DataLayer.TripService.Contracts;
using SmartCity.DataLayer.TripService.DataContracts;
using SmartCity.DataLayer.TripService.Repositories;

namespace SmartCity.DataLayer.TripService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class TripDataService : ITripDataService
{
    private readonly TripRepository _repository;
    private readonly ILogger<TripDataService> _logger;

    public TripDataService(TripRepository repository, ILogger<TripDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TripDataContract?> GetTripByIdAsync(Guid tripId)
    {
        _logger.LogInformation("SOAP: GetTripById {TripId}", tripId);
        return await _repository.GetByIdAsync(tripId);
    }

    public async Task<TripDataContract?> GetActiveTripByUserAsync(Guid userId)
    {
        _logger.LogInformation("SOAP: GetActiveTripByUser {UserId}", userId);
        return await _repository.GetActiveTripByUserAsync(userId);
    }

    public async Task<TripDataContract[]> GetTripsByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetTripsByUser {UserId} Page={Page} Size={Size}", userId, pageNumber, pageSize);
        return await _repository.GetByUserAsync(userId, pageNumber, pageSize);
    }

    public async Task<TripDataContract[]> GetTripsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetTripsByVehicle {VehicleId} Page={Page} Size={Size}", vehicleId, pageNumber, pageSize);
        return await _repository.GetByVehicleAsync(vehicleId, pageNumber, pageSize);
    }

    public async Task<TripDataContract[]> GetAllTripsAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAllTrips Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<TripDataContract> CreateTripAsync(CreateTripRequest request)
    {
        _logger.LogInformation("SOAP: CreateTrip User={UserId} Vehicle={VehicleId}", request.UserId, request.VehicleId);
        return await _repository.CreateAsync(request);
    }

    public async Task<TripDataContract?> EndTripAsync(Guid tripId, EndTripRequest request)
    {
        _logger.LogInformation("SOAP: EndTrip {TripId}", tripId);
        return await _repository.EndAsync(tripId, request);
    }

    public async Task<bool> CancelTripAsync(Guid tripId, string reason)
    {
        _logger.LogInformation("SOAP: CancelTrip {TripId} Reason={Reason}", tripId, reason);
        return await _repository.CancelAsync(tripId, reason);
    }

    public async Task<bool> AddTripLocationAsync(Guid tripId, decimal latitude, decimal longitude, int? speed, int? batteryLevel)
    {
        _logger.LogInformation("SOAP: AddTripLocation {TripId}", tripId);
        return await _repository.AddLocationAsync(tripId, latitude, longitude, speed, batteryLevel);
    }

    public async Task<TripLocationDataContract[]> GetTripLocationsAsync(Guid tripId)
    {
        _logger.LogInformation("SOAP: GetTripLocations {TripId}", tripId);
        return await _repository.GetLocationsAsync(tripId);
    }

    public async Task<bool> RateTripAsync(Guid tripId, int rating, string? review)
    {
        _logger.LogInformation("SOAP: RateTrip {TripId} Rating={Rating}", tripId, rating);
        return await _repository.RateAsync(tripId, rating, review);
    }

    public async Task<TripStatisticsDataContract> GetUserStatisticsAsync(Guid userId, DateTime? startDate, DateTime? endDate)
    {
        _logger.LogInformation("SOAP: GetUserStatistics {UserId}", userId);
        return await _repository.GetUserStatisticsAsync(userId, startDate, endDate);
    }

    public async Task<TripStatisticsDataContract> GetVehicleStatisticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate)
    {
        _logger.LogInformation("SOAP: GetVehicleStatistics {VehicleId}", vehicleId);
        return await _repository.GetVehicleStatisticsAsync(vehicleId, startDate, endDate);
    }

    public async Task<TripStatisticsDataContract> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate)
    {
        _logger.LogInformation("SOAP: GetSystemStatistics");
        return await _repository.GetSystemStatisticsAsync(startDate, endDate);
    }
}