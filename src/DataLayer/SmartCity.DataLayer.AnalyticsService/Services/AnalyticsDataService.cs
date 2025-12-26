using CoreWCF;
using SmartCity.DataLayer.AnalyticsService.Contracts;
using SmartCity.DataLayer.AnalyticsService.DataContracts;
using SmartCity.DataLayer.AnalyticsService.Repositories;

namespace SmartCity.DataLayer.AnalyticsService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class AnalyticsDataService : IAnalyticsDataService
{
    private readonly AnalyticsRepository _repository;
    private readonly ILogger<AnalyticsDataService> _logger;

    public AnalyticsDataService(AnalyticsRepository repository, ILogger<AnalyticsDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<VehicleAnalyticsDataContract?> GetVehicleAnalyticsAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetVehicleAnalytics {VehicleId} From={Start} To={End}", 
            vehicleId, startDate, endDate);
        return await _repository.GetVehicleAnalyticsAsync(vehicleId, startDate, endDate);
    }

    public async Task<VehicleAnalyticsDataContract[]> GetTopPerformingVehiclesAsync(DateTime startDate, DateTime endDate, int limit)
    {
        _logger.LogInformation("SOAP: GetTopPerformingVehicles From={Start} To={End} Limit={Limit}", 
            startDate, endDate, limit);
        return await _repository.GetTopPerformingVehiclesAsync(startDate, endDate, limit);
    }

    public async Task<UserAnalyticsDataContract?> GetUserAnalyticsAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetUserAnalytics {UserId} From={Start} To={End}", 
            userId, startDate, endDate);
        return await _repository.GetUserAnalyticsAsync(userId, startDate, endDate);
    }

    public async Task<UserAnalyticsDataContract[]> GetTopUsersAsync(DateTime startDate, DateTime endDate, int limit)
    {
        _logger.LogInformation("SOAP: GetTopUsers From={Start} To={End} Limit={Limit}", 
            startDate, endDate, limit);
        return await _repository.GetTopUsersAsync(startDate, endDate, limit);
    }

    public async Task<SystemAnalyticsDataContract> GetSystemAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetSystemAnalytics From={Start} To={End}", startDate, endDate);
        return await _repository.GetSystemAnalyticsAsync(startDate, endDate);
    }

    public async Task<RevenueAnalyticsDataContract[]> GetRevenueByDateAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetRevenueByDate From={Start} To={End}", startDate, endDate);
        return await _repository.GetRevenueByDateAsync(startDate, endDate);
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetTotalRevenue From={Start} To={End}", startDate, endDate);
        return await _repository.GetTotalRevenueAsync(startDate, endDate);
    }

    public async Task<PopularRouteDataContract[]> GetPopularRoutesAsync(DateTime startDate, DateTime endDate, int limit)
    {
        _logger.LogInformation("SOAP: GetPopularRoutes From={Start} To={End} Limit={Limit}", 
            startDate, endDate, limit);
        return await _repository.GetPopularRoutesAsync(startDate, endDate, limit);
    }

    public async Task<VehicleTypeAnalyticsDataContract[]> GetVehicleTypeAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetVehicleTypeAnalytics From={Start} To={End}", startDate, endDate);
        return await _repository.GetVehicleTypeAnalyticsAsync(startDate, endDate);
    }

    public async Task<HourlyUsageDataContract[]> GetHourlyUsageAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetHourlyUsage From={Start} To={End}", startDate, endDate);
        return await _repository.GetHourlyUsageAsync(startDate, endDate);
    }

    public async Task<StationAnalyticsDataContract[]> GetStationAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetStationAnalytics From={Start} To={End}", startDate, endDate);
        return await _repository.GetStationAnalyticsAsync(startDate, endDate);
    }

    public async Task<StationAnalyticsDataContract?> GetStationAnalyticsByIdAsync(Guid stationId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("SOAP: GetStationAnalyticsById {StationId} From={Start} To={End}", 
            stationId, startDate, endDate);
        return await _repository.GetStationAnalyticsByIdAsync(stationId, startDate, endDate);
    }
}