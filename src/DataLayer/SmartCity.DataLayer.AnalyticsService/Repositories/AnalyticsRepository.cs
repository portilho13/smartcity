using Dapper;
using SmartCity.DataLayer.AnalyticsService.DataContracts;
using SmartCity.DataLayer.AnalyticsService.Infrastructure;
using System.Data;

namespace SmartCity.DataLayer.AnalyticsService.Repositories;

public class AnalyticsRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<AnalyticsRepository> _logger;

    public AnalyticsRepository(DatabaseConnectionFactory connectionFactory, ILogger<AnalyticsRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // ==================== Vehicle Analytics ====================

    public async Task<VehicleAnalyticsDataContract?> GetVehicleAnalyticsAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                @VehicleId as VehicleId,
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COALESCE(AVG(rating), 0) as AverageRating,
                COUNT(rating) as TotalRatings,
                CASE 
                    WHEN COUNT(*) > 0 THEN 
                        (CAST(SUM(duration_minutes) AS DECIMAL) / (EXTRACT(EPOCH FROM (@EndDate - @StartDate)) / 60)) * 100
                    ELSE 0
                END as UtilizationRate,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM trips
            WHERE vehicle_id = @VehicleId
              AND start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<VehicleAnalyticsDataContract>(
            sql,
            new { VehicleId = vehicleId, StartDate = startDate, EndDate = endDate }
        );
    }

    public async Task<VehicleAnalyticsDataContract[]> GetTopPerformingVehiclesAsync(DateTime startDate, DateTime endDate, int limit)
    {
        const string sql = @"
            SELECT 
                vehicle_id as VehicleId,
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COALESCE(AVG(rating), 0) as AverageRating,
                COUNT(rating) as TotalRatings,
                CASE 
                    WHEN COUNT(*) > 0 THEN 
                        (CAST(SUM(duration_minutes) AS DECIMAL) / (EXTRACT(EPOCH FROM (@EndDate - @StartDate)) / 60)) * 100
                    ELSE 0
                END as UtilizationRate,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')
            GROUP BY vehicle_id
            ORDER BY TotalRevenue DESC
            LIMIT @Limit";

        using var connection = _connectionFactory.CreateConnection();
        var vehicles = await connection.QueryAsync<VehicleAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate, Limit = limit }
        );
        return vehicles.ToArray();
    }

    // ==================== User Analytics ====================

    public async Task<UserAnalyticsDataContract?> GetUserAnalyticsAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                @UserId as UserId,
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalSpent,
                COALESCE(AVG(rating), 0) as AverageRating,
                MAX(start_time) as LastTripDate,
                (
                    SELECT vt.name
                    FROM trips t
                    JOIN vehicles v ON t.vehicle_id = v.id
                    JOIN vehicle_types vt ON v.vehicle_type_id = vt.id
                    WHERE t.user_id = @UserId
                      AND t.start_time >= @StartDate
                      AND t.start_time <= @EndDate
                    GROUP BY vt.name
                    ORDER BY COUNT(*) DESC
                    LIMIT 1
                ) as FavoriteVehicleType,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM trips
            WHERE user_id = @UserId
              AND start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserAnalyticsDataContract>(
            sql,
            new { UserId = userId, StartDate = startDate, EndDate = endDate }
        );
    }

    public async Task<UserAnalyticsDataContract[]> GetTopUsersAsync(DateTime startDate, DateTime endDate, int limit)
    {
        const string sql = @"
            SELECT 
                user_id as UserId,
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalSpent,
                COALESCE(AVG(rating), 0) as AverageRating,
                MAX(start_time) as LastTripDate,
                NULL as FavoriteVehicleType,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')
            GROUP BY user_id
            ORDER BY TotalSpent DESC
            LIMIT @Limit";

        using var connection = _connectionFactory.CreateConnection();
        var users = await connection.QueryAsync<UserAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate, Limit = limit }
        );
        return users.ToArray();
    }

    // ==================== System Analytics ====================

    public async Task<SystemAnalyticsDataContract> GetSystemAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                (SELECT COUNT(*) FROM users WHERE created_at <= @EndDate) as TotalUsers,
                (SELECT COUNT(DISTINCT user_id) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate) as ActiveUsers,
                (SELECT COUNT(*) FROM vehicles WHERE created_at <= @EndDate AND retired_at IS NULL) as TotalVehicles,
                (SELECT COUNT(*) FROM vehicles WHERE status = 'available' AND retired_at IS NULL) as AvailableVehicles,
                (SELECT COUNT(*) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate) as TotalTrips,
                (SELECT COUNT(*) FROM trips WHERE status = 'active') as ActiveTrips,
                (SELECT COALESCE(SUM(total_fare), 0) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate AND status IN ('completed', 'cancelled')) as TotalRevenue,
                (SELECT COALESCE(SUM(distance_km), 0) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate AND status IN ('completed', 'cancelled')) as TotalDistanceKm,
                (SELECT COALESCE(AVG(rating), 0) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate AND rating IS NOT NULL) as AverageRating,
                (SELECT COALESCE(AVG(duration_minutes), 0) FROM trips WHERE start_time >= @StartDate AND start_time <= @EndDate AND status IN ('completed', 'cancelled')) as AverageTripDuration,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SystemAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
    }

    // ==================== Revenue Analytics ====================

    public async Task<RevenueAnalyticsDataContract[]> GetRevenueByDateAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                DATE(start_time) as Date,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COUNT(*) as TripCount,
                COALESCE(AVG(total_fare), 0) as AverageRevenuePerTrip
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')
            GROUP BY DATE(start_time)
            ORDER BY Date";

        using var connection = _connectionFactory.CreateConnection();
        var revenue = await connection.QueryAsync<RevenueAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
        return revenue.ToArray();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT COALESCE(SUM(total_fare), 0)
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<decimal>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
    }

    // ==================== Route Analytics ====================

    public async Task<PopularRouteDataContract[]> GetPopularRoutesAsync(DateTime startDate, DateTime endDate, int limit)
    {
        const string sql = @"
            SELECT 
                CONCAT('Route-', ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC)) as RouteName,
                start_latitude as StartLatitude,
                start_longitude as StartLongitude,
                end_latitude as EndLatitude,
                end_longitude as EndLongitude,
                COUNT(*) as TripCount,
                AVG(distance_km) as AverageDistance,
                AVG(duration_minutes) as AverageDuration
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status = 'completed'
              AND start_latitude IS NOT NULL
              AND start_longitude IS NOT NULL
              AND end_latitude IS NOT NULL
              AND end_longitude IS NOT NULL
            GROUP BY 
                ROUND(CAST(start_latitude AS NUMERIC), 3),
                ROUND(CAST(start_longitude AS NUMERIC), 3),
                ROUND(CAST(end_latitude AS NUMERIC), 3),
                ROUND(CAST(end_longitude AS NUMERIC), 3)
            HAVING COUNT(*) >= 3
            ORDER BY TripCount DESC
            LIMIT @Limit";

        using var connection = _connectionFactory.CreateConnection();
        var routes = await connection.QueryAsync<PopularRouteDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate, Limit = limit }
        );
        return routes.ToArray();
    }

    // ==================== Vehicle Type Analytics ====================

    public async Task<VehicleTypeAnalyticsDataContract[]> GetVehicleTypeAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                vt.id as VehicleTypeId,
                vt.name as VehicleTypeName,
                COUNT(DISTINCT v.id) as TotalVehicles,
                COUNT(t.id) as TotalTrips,
                COALESCE(SUM(t.total_fare), 0) as TotalRevenue,
                COALESCE(AVG(t.rating), 0) as AverageRating,
                CASE 
                    WHEN COUNT(t.id) > 0 THEN 
                        (CAST(SUM(t.duration_minutes) AS DECIMAL) / (COUNT(DISTINCT v.id) * EXTRACT(EPOCH FROM (@EndDate - @StartDate)) / 60)) * 100
                    ELSE 0
                END as UtilizationRate
            FROM vehicle_types vt
            LEFT JOIN vehicles v ON vt.id = v.vehicle_type_id AND v.retired_at IS NULL
            LEFT JOIN trips t ON v.id = t.vehicle_id 
                AND t.start_time >= @StartDate 
                AND t.start_time <= @EndDate
                AND t.status IN ('completed', 'cancelled')
            GROUP BY vt.id, vt.name
            ORDER BY TotalRevenue DESC";

        using var connection = _connectionFactory.CreateConnection();
        var types = await connection.QueryAsync<VehicleTypeAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
        return types.ToArray();
    }

    // ==================== Usage Analytics ====================

    public async Task<HourlyUsageDataContract[]> GetHourlyUsageAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                EXTRACT(HOUR FROM start_time) as Hour,
                COUNT(*) as TripCount,
                COALESCE(SUM(total_fare), 0) as Revenue,
                COUNT(DISTINCT user_id) as UniqueUsers
            FROM trips
            WHERE start_time >= @StartDate
              AND start_time <= @EndDate
              AND status IN ('completed', 'cancelled')
            GROUP BY EXTRACT(HOUR FROM start_time)
            ORDER BY Hour";

        using var connection = _connectionFactory.CreateConnection();
        var usage = await connection.QueryAsync<HourlyUsageDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
        return usage.ToArray();
    }

    // ==================== Station Analytics ====================

    public async Task<StationAnalyticsDataContract[]> GetStationAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                s.id as StationId,
                s.name as StationName,
                (SELECT COUNT(*) FROM trips WHERE start_station_id = s.id AND start_time >= @StartDate AND start_time <= @EndDate) as TripsStarted,
                (SELECT COUNT(*) FROM trips WHERE end_station_id = s.id AND end_time >= @StartDate AND end_time <= @EndDate) as TripsEnded,
                CAST((s.total_capacity - s.available_slots) AS DECIMAL) / NULLIF(s.total_capacity, 0) * 100 as AverageOccupancy,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM stations s
            ORDER BY TripsStarted DESC";

        using var connection = _connectionFactory.CreateConnection();
        var stations = await connection.QueryAsync<StationAnalyticsDataContract>(
            sql,
            new { StartDate = startDate, EndDate = endDate }
        );
        return stations.ToArray();
    }

    public async Task<StationAnalyticsDataContract?> GetStationAnalyticsByIdAsync(Guid stationId, DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                s.id as StationId,
                s.name as StationName,
                (SELECT COUNT(*) FROM trips WHERE start_station_id = s.id AND start_time >= @StartDate AND start_time <= @EndDate) as TripsStarted,
                (SELECT COUNT(*) FROM trips WHERE end_station_id = s.id AND end_time >= @StartDate AND end_time <= @EndDate) as TripsEnded,
                CAST((s.total_capacity - s.available_slots) AS DECIMAL) / NULLIF(s.total_capacity, 0) * 100 as AverageOccupancy,
                @StartDate as PeriodStart,
                @EndDate as PeriodEnd
            FROM stations s
            WHERE s.id = @StationId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<StationAnalyticsDataContract>(
            sql,
            new { StationId = stationId, StartDate = startDate, EndDate = endDate }
        );
    }
}