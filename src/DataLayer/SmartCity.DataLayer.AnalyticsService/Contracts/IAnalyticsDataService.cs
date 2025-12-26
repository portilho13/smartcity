using System.ServiceModel;
using SmartCity.DataLayer.AnalyticsService.DataContracts;

namespace SmartCity.DataLayer.AnalyticsService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public interface IAnalyticsDataService
{
    // Vehicle Analytics
    [OperationContract]
    Task<VehicleAnalyticsDataContract?> GetVehicleAnalyticsAsync(Guid vehicleId, DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<VehicleAnalyticsDataContract[]> GetTopPerformingVehiclesAsync(DateTime startDate, DateTime endDate, int limit);

    // User Analytics
    [OperationContract]
    Task<UserAnalyticsDataContract?> GetUserAnalyticsAsync(Guid userId, DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<UserAnalyticsDataContract[]> GetTopUsersAsync(DateTime startDate, DateTime endDate, int limit);

    // System Analytics
    [OperationContract]
    Task<SystemAnalyticsDataContract> GetSystemAnalyticsAsync(DateTime startDate, DateTime endDate);

    // Revenue Analytics
    [OperationContract]
    Task<RevenueAnalyticsDataContract[]> GetRevenueByDateAsync(DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);

    // Route Analytics
    [OperationContract]
    Task<PopularRouteDataContract[]> GetPopularRoutesAsync(DateTime startDate, DateTime endDate, int limit);

    // Vehicle Type Analytics
    [OperationContract]
    Task<VehicleTypeAnalyticsDataContract[]> GetVehicleTypeAnalyticsAsync(DateTime startDate, DateTime endDate);

    // Usage Analytics
    [OperationContract]
    Task<HourlyUsageDataContract[]> GetHourlyUsageAsync(DateTime startDate, DateTime endDate);

    // Station Analytics
    [OperationContract]
    Task<StationAnalyticsDataContract[]> GetStationAnalyticsAsync(DateTime startDate, DateTime endDate);

    [OperationContract]
    Task<StationAnalyticsDataContract?> GetStationAnalyticsByIdAsync(Guid stationId, DateTime startDate, DateTime endDate);
}