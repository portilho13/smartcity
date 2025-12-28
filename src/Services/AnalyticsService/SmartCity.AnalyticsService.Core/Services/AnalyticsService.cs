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

using SmartCity.AnalyticsService.Core.DTOs;
using SmartCity.DataLayer.AnalyticsService.Contracts;
using SmartCity.DataLayer.AnalyticsService.DataContracts;
using Microsoft.Extensions.Logging;

namespace SmartCity.AnalyticsService.Core.Services;

public interface IAnalyticsService
{
    Task<VehicleAnalyticsDto?> GetVehicleAnalyticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<VehicleAnalyticsDto>> GetTopPerformingVehiclesAsync(DateTime? startDate, DateTime? endDate, int limit);
    
    Task<UserAnalyticsDto?> GetUserAnalyticsAsync(Guid userId, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<UserAnalyticsDto>> GetTopUsersAsync(DateTime? startDate, DateTime? endDate, int limit);
    
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync(DateTime? startDate, DateTime? endDate);
    
    Task<IEnumerable<RevenueAnalyticsDto>> GetRevenueByDateAsync(DateTime? startDate, DateTime? endDate);
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate);
    
    Task<IEnumerable<PopularRouteDto>> GetPopularRoutesAsync(DateTime? startDate, DateTime? endDate, int limit);
    
    Task<IEnumerable<VehicleTypeAnalyticsDto>> GetVehicleTypeAnalyticsAsync(DateTime? startDate, DateTime? endDate);
    
    Task<IEnumerable<HourlyUsageDto>> GetHourlyUsageAsync(DateTime? startDate, DateTime? endDate);
    
    Task<IEnumerable<StationAnalyticsDto>> GetStationAnalyticsAsync(DateTime? startDate, DateTime? endDate);
    Task<StationAnalyticsDto?> GetStationAnalyticsByIdAsync(Guid stationId, DateTime? startDate, DateTime? endDate);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsDataService _soapClient;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(IAnalyticsDataService soapClient, ILogger<AnalyticsService> logger)
    {
        _soapClient = soapClient;
        _logger = logger;
    }

    private (DateTime start, DateTime end) GetDateRange(DateTime? startDate, DateTime? endDate)
    {
        var end = endDate ?? DateTime.UtcNow;
        var start = startDate ?? end.AddDays(-30);
        return (start, end);
    }

    public async Task<VehicleAnalyticsDto?> GetVehicleAnalyticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetVehicleAnalyticsAsync(vehicleId, start, end);
        return analyticsData != null ? MapToDto(analyticsData) : null;
    }

    public async Task<IEnumerable<VehicleAnalyticsDto>> GetTopPerformingVehiclesAsync(DateTime? startDate, DateTime? endDate, int limit)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetTopPerformingVehiclesAsync(start, end, limit);
        return analyticsData.Select(MapToDto);
    }

    public async Task<UserAnalyticsDto?> GetUserAnalyticsAsync(Guid userId, DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetUserAnalyticsAsync(userId, start, end);
        return analyticsData != null ? MapToDto(analyticsData) : null;
    }

    public async Task<IEnumerable<UserAnalyticsDto>> GetTopUsersAsync(DateTime? startDate, DateTime? endDate, int limit)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetTopUsersAsync(start, end, limit);
        return analyticsData.Select(MapToDto);
    }

    public async Task<SystemAnalyticsDto> GetSystemAnalyticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetSystemAnalyticsAsync(start, end);
        return MapToDto(analyticsData);
    }

    public async Task<IEnumerable<RevenueAnalyticsDto>> GetRevenueByDateAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetRevenueByDateAsync(start, end);
        return analyticsData.Select(MapToDto);
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        return await _soapClient.GetTotalRevenueAsync(start, end);
    }

    public async Task<IEnumerable<PopularRouteDto>> GetPopularRoutesAsync(DateTime? startDate, DateTime? endDate, int limit)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetPopularRoutesAsync(start, end, limit);
        return analyticsData.Select(MapToDto);
    }

    public async Task<IEnumerable<VehicleTypeAnalyticsDto>> GetVehicleTypeAnalyticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetVehicleTypeAnalyticsAsync(start, end);
        return analyticsData.Select(MapToDto);
    }

    public async Task<IEnumerable<HourlyUsageDto>> GetHourlyUsageAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetHourlyUsageAsync(start, end);
        return analyticsData.Select(MapToDto);
    }

    public async Task<IEnumerable<StationAnalyticsDto>> GetStationAnalyticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetStationAnalyticsAsync(start, end);
        return analyticsData.Select(MapToDto);
    }

    public async Task<StationAnalyticsDto?> GetStationAnalyticsByIdAsync(Guid stationId, DateTime? startDate, DateTime? endDate)
    {
        var (start, end) = GetDateRange(startDate, endDate);
        var analyticsData = await _soapClient.GetStationAnalyticsByIdAsync(stationId, start, end);
        return analyticsData != null ? MapToDto(analyticsData) : null;
    }

    // Mapping methods
    private static VehicleAnalyticsDto MapToDto(VehicleAnalyticsDataContract data)
    {
        return new VehicleAnalyticsDto
        {
            VehicleId = data.VehicleId,
            TotalTrips = data.TotalTrips,
            TotalDistanceKm = data.TotalDistanceKm,
            TotalDurationMinutes = data.TotalDurationMinutes,
            TotalRevenue = data.TotalRevenue,
            AverageRating = data.AverageRating,
            TotalRatings = data.TotalRatings,
            UtilizationRate = data.UtilizationRate,
            PeriodStart = data.PeriodStart,
            PeriodEnd = data.PeriodEnd
        };
    }

    private static UserAnalyticsDto MapToDto(UserAnalyticsDataContract data)
    {
        return new UserAnalyticsDto
        {
            UserId = data.UserId,
            TotalTrips = data.TotalTrips,
            TotalDistanceKm = data.TotalDistanceKm,
            TotalDurationMinutes = data.TotalDurationMinutes,
            TotalSpent = data.TotalSpent,
            AverageRating = data.AverageRating,
            LastTripDate = data.LastTripDate,
            FavoriteVehicleType = data.FavoriteVehicleType,
            PeriodStart = data.PeriodStart,
            PeriodEnd = data.PeriodEnd
        };
    }

    private static SystemAnalyticsDto MapToDto(SystemAnalyticsDataContract data)
    {
        return new SystemAnalyticsDto
        {
            TotalUsers = data.TotalUsers,
            ActiveUsers = data.ActiveUsers,
            TotalVehicles = data.TotalVehicles,
            AvailableVehicles = data.AvailableVehicles,
            TotalTrips = data.TotalTrips,
            ActiveTrips = data.ActiveTrips,
            TotalRevenue = data.TotalRevenue,
            TotalDistanceKm = data.TotalDistanceKm,
            AverageRating = data.AverageRating,
            AverageTripDuration = data.AverageTripDuration,
            PeriodStart = data.PeriodStart,
            PeriodEnd = data.PeriodEnd
        };
    }

    private static RevenueAnalyticsDto MapToDto(RevenueAnalyticsDataContract data)
    {
        return new RevenueAnalyticsDto
        {
            Date = data.Date,
            TotalRevenue = data.TotalRevenue,
            TripCount = data.TripCount,
            AverageRevenuePerTrip = data.AverageRevenuePerTrip
        };
    }

    private static PopularRouteDto MapToDto(PopularRouteDataContract data)
    {
        return new PopularRouteDto
        {
            RouteName = data.RouteName,
            StartLatitude = data.StartLatitude,
            StartLongitude = data.StartLongitude,
            EndLatitude = data.EndLatitude,
            EndLongitude = data.EndLongitude,
            TripCount = data.TripCount,
            AverageDistance = data.AverageDistance,
            AverageDuration = data.AverageDuration
        };
    }

    private static VehicleTypeAnalyticsDto MapToDto(VehicleTypeAnalyticsDataContract data)
    {
        return new VehicleTypeAnalyticsDto
        {
            VehicleTypeId = data.VehicleTypeId,
            VehicleTypeName = data.VehicleTypeName,
            TotalVehicles = data.TotalVehicles,
            TotalTrips = data.TotalTrips,
            TotalRevenue = data.TotalRevenue,
            AverageRating = data.AverageRating,
            UtilizationRate = data.UtilizationRate
        };
    }

    private static HourlyUsageDto MapToDto(HourlyUsageDataContract data)
    {
        return new HourlyUsageDto
        {
            Hour = data.Hour,
            TripCount = data.TripCount,
            Revenue = data.Revenue,
            UniqueUsers = data.UniqueUsers
        };
    }

    private static StationAnalyticsDto MapToDto(StationAnalyticsDataContract data)
    {
        return new StationAnalyticsDto
        {
            StationId = data.StationId,
            StationName = data.StationName,
            TripsStarted = data.TripsStarted,
            TripsEnded = data.TripsEnded,
            AverageOccupancy = data.AverageOccupancy,
            PeriodStart = data.PeriodStart,
            PeriodEnd = data.PeriodEnd
        };
    }
}