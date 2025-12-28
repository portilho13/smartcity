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

using SmartCity.TripManagement.Core.DTOs;
using SmartCity.DataLayer.TripService.Contracts;
using SmartCity.DataLayer.TripService.DataContracts;
using Microsoft.Extensions.Logging;
using EndTripRequest = SmartCity.TripManagement.Core.DTOs.EndTripRequest;

namespace SmartCity.TripManagement.Core.Services;

public interface ITripService
{
    Task<TripDto?> GetTripByIdAsync(Guid tripId);
    Task<TripDto?> GetActiveTripByUserAsync(Guid userId);
    Task<IEnumerable<TripDto>> GetTripsByUserAsync(Guid userId, int page, int pageSize);
    Task<IEnumerable<TripDto>> GetTripsByVehicleAsync(Guid vehicleId, int page, int pageSize);
    Task<IEnumerable<TripDto>> GetAllTripsAsync(int page, int pageSize);
    Task<TripDto> StartTripAsync(Guid userId, StartTripWithPriceRequest request);
    Task<TripDto?> EndTripAsync(Guid tripId, EndTripRequest request);
    Task<bool> CancelTripAsync(Guid tripId, string reason);
    Task<bool> AddTripLocationAsync(Guid tripId, AddTripLocationRequest request);
    Task<IEnumerable<TripLocationDto>> GetTripLocationsAsync(Guid tripId);
    Task<bool> RateTripAsync(Guid tripId, int rating, string? review);
    Task<TripStatisticsDto> GetUserStatisticsAsync(Guid userId, DateTime? startDate, DateTime? endDate);
    Task<TripStatisticsDto> GetVehicleStatisticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate);
    Task<TripStatisticsDto> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate);
}

public class TripService : ITripService
{
    private readonly ITripDataService _soapClient;
    private readonly ILogger<TripService> _logger;

    public TripService(ITripDataService soapClient, ILogger<TripService> logger)
    {
        _soapClient = soapClient;
        _logger = logger;
    }

    public async Task<TripDto?> GetTripByIdAsync(Guid tripId)
    {
        var tripData = await _soapClient.GetTripByIdAsync(tripId);
        if (tripData == null)
            throw new KeyNotFoundException($"Trip with ID {tripId} not found");

        return MapToDto(tripData);
    }

    public async Task<TripDto?> GetActiveTripByUserAsync(Guid userId)
    {
        var tripData = await _soapClient.GetActiveTripByUserAsync(userId);
        return tripData != null ? MapToDto(tripData) : null;
    }

    public async Task<IEnumerable<TripDto>> GetTripsByUserAsync(Guid userId, int page, int pageSize)
    {
        var tripsData = await _soapClient.GetTripsByUserAsync(userId, page, pageSize);
        return tripsData.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetTripsByVehicleAsync(Guid vehicleId, int page, int pageSize)
    {
        var tripsData = await _soapClient.GetTripsByVehicleAsync(vehicleId, page, pageSize);
        return tripsData.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetAllTripsAsync(int page, int pageSize)
    {
        var tripsData = await _soapClient.GetAllTripsAsync(page, pageSize);
        return tripsData.Select(MapToDto);
    }

    public async Task<TripDto> StartTripAsync(Guid userId, StartTripWithPriceRequest request)
    {
        var createRequest = new CreateTripRequest
        {
            UserId = userId,
            VehicleId = request.VehicleId,
            StartLatitude = request.StartLatitude,
            StartLongitude = request.StartLongitude,
            StartStationId = request.StartStationId,
            UnlockFee = request.UnlockFee,
            RatePerMinute = request.RatePerMinute,
        };

        var tripData = await _soapClient.CreateTripAsync(createRequest);
        return MapToDto(tripData);
    }

    public async Task<TripDto?> EndTripAsync(Guid tripId, EndTripRequest request)
    {
        var endRequest = new SmartCity.DataLayer.TripService.DataContracts.EndTripRequest
        {
            EndLatitude = request.EndLatitude,
            EndLongitude = request.EndLongitude,
            EndStationId = request.EndStationId
        };

        var tripData = await _soapClient.EndTripAsync(tripId, endRequest);
        return tripData != null ? MapToDto(tripData) : null;
    }

    public async Task<bool> CancelTripAsync(Guid tripId, string reason)
    {
        return await _soapClient.CancelTripAsync(tripId, reason);
    }

    public async Task<bool> AddTripLocationAsync(Guid tripId, AddTripLocationRequest request)
    {
        return await _soapClient.AddTripLocationAsync(
            tripId, 
            request.Latitude, 
            request.Longitude, 
            request.Speed, 
            request.BatteryLevel
        );
    }

    public async Task<IEnumerable<TripLocationDto>> GetTripLocationsAsync(Guid tripId)
    {
        var locationsData = await _soapClient.GetTripLocationsAsync(tripId);
        return locationsData.Select(MapToDto);
    }

    public async Task<bool> RateTripAsync(Guid tripId, int rating, string? review)
    {
        return await _soapClient.RateTripAsync(tripId, rating, review);
    }

    public async Task<TripStatisticsDto> GetUserStatisticsAsync(Guid userId, DateTime? startDate, DateTime? endDate)
    {
        var statsData = await _soapClient.GetUserStatisticsAsync(userId, startDate, endDate);
        return MapToDto(statsData);
    }

    public async Task<TripStatisticsDto> GetVehicleStatisticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate)
    {
        var statsData = await _soapClient.GetVehicleStatisticsAsync(vehicleId, startDate, endDate);
        return MapToDto(statsData);
    }

    public async Task<TripStatisticsDto> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var statsData = await _soapClient.GetSystemStatisticsAsync(startDate, endDate);
        return MapToDto(statsData);
    }

    private static TripDto MapToDto(TripDataContract data)
    {
        return new TripDto
        {
            Id = data.Id,
            UserId = data.UserId,
            VehicleId = data.VehicleId,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            StartLatitude = data.StartLatitude,
            StartLongitude = data.StartLongitude,
            EndLatitude = data.EndLatitude,
            EndLongitude = data.EndLongitude,
            StartStationId = data.StartStationId,
            EndStationId = data.EndStationId,
            DistanceKm = data.DistanceKm,
            DurationMinutes = data.DurationMinutes,
            BaseFare = data.BaseFare,
            DistanceFare = data.DistanceFare,
            TimeFare = data.TimeFare,
            TotalFare = data.TotalFare,
            Status = data.Status,
            CancellationReason = data.CancellationReason,
            Rating = data.Rating,
            Review = data.Review,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt
        };
    }

    private static TripLocationDto MapToDto(TripLocationDataContract data)
    {
        return new TripLocationDto
        {
            Id = data.Id,
            TripId = data.TripId,
            Latitude = data.Latitude,
            Longitude = data.Longitude,
            Speed = data.Speed,
            BatteryLevel = data.BatteryLevel,
            Timestamp = data.Timestamp
        };
    }

    private static TripStatisticsDto MapToDto(TripStatisticsDataContract data)
    {
        return new TripStatisticsDto
        {
            TotalTrips = data.TotalTrips,
            TotalDistanceKm = data.TotalDistanceKm,
            TotalDurationMinutes = data.TotalDurationMinutes,
            TotalRevenue = data.TotalRevenue,
            AverageRating = data.AverageRating,
            PeriodStart = data.PeriodStart,
            PeriodEnd = data.PeriodEnd
        };
    }
}