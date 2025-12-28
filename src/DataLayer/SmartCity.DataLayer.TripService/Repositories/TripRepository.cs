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

using Dapper;
using SmartCity.DataLayer.TripService.DataContracts;
using SmartCity.DataLayer.TripService.Infrastructure;

namespace SmartCity.DataLayer.TripService.Repositories;

public class TripRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<TripRepository> _logger;

    public TripRepository(DatabaseConnectionFactory connectionFactory, ILogger<TripRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<TripDataContract?> GetByIdAsync(Guid tripId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt
            FROM trips
            WHERE id = @TripId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TripDataContract>(sql, new { TripId = tripId });
    }

    public async Task<TripDataContract?> GetActiveTripByUserAsync(Guid userId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt
            FROM trips
            WHERE user_id = @UserId AND status = 'active'
            ORDER BY start_time DESC
            LIMIT 1";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TripDataContract>(sql, new { UserId = userId });
    }

    public async Task<TripDataContract[]> GetByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt
            FROM trips
            WHERE user_id = @UserId
            ORDER BY start_time DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var trips = await connection.QueryAsync<TripDataContract>(sql, new { UserId = userId, PageSize = pageSize, Offset = offset });
        return trips.ToArray();
    }

    public async Task<TripDataContract[]> GetByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt
            FROM trips
            WHERE vehicle_id = @VehicleId
            ORDER BY start_time DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var trips = await connection.QueryAsync<TripDataContract>(sql, new { VehicleId = vehicleId, PageSize = pageSize, Offset = offset });
        return trips.ToArray();
    }

    public async Task<TripDataContract[]> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt
            FROM trips
            ORDER BY start_time DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var trips = await connection.QueryAsync<TripDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return trips.ToArray();
    }

    public async Task<TripDataContract> CreateAsync(CreateTripRequest request)
    {
        const string sql = @"
        INSERT INTO trips (
            user_id, vehicle_id, start_time, 
            start_latitude, start_longitude, start_station_id,
            unlock_fee, rate_per_minute,
            status, created_at, updated_at
        )
        VALUES (
            @UserId, @VehicleId, CURRENT_TIMESTAMP,
            @StartLatitude, @StartLongitude, @StartStationId,
            @UnlockFee, @RatePerMinute,
            'active', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
        )
        RETURNING 
            id, user_id as UserId, vehicle_id as VehicleId,
            start_time as StartTime, end_time as EndTime,
            start_latitude as StartLatitude, start_longitude as StartLongitude,
            end_latitude as EndLatitude, end_longitude as EndLongitude,
            start_station_id as StartStationId, end_station_id as EndStationId,
            distance_km as DistanceKm, duration_minutes as DurationMinutes,
            unlock_fee as UnlockFee, rate_per_minute as RatePerMinute,
            base_fare as BaseFare, distance_fare as DistanceFare,
            time_fare as TimeFare, total_fare as TotalFare,
            status, cancellation_reason as CancellationReason,
            rating, review, created_at as CreatedAt, updated_at as UpdatedAt";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<TripDataContract>(sql, request);
    }

    public async Task<TripDataContract?> EndAsync(Guid tripId, EndTripRequest request)
    {
        const string sql = @"
            UPDATE trips
            SET 
                end_time = CURRENT_TIMESTAMP,
                end_latitude = @EndLatitude,
                end_longitude = @EndLongitude,
                end_station_id = @EndStationId,
                distance_km = earth_distance(
                    ll_to_earth(start_latitude, start_longitude),
                    ll_to_earth(@EndLatitude, @EndLongitude)
                ) / 1000.0,
                duration_minutes = CEIL(EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - start_time)) / 60)::integer,
                base_fare = unlock_fee,
                distance_fare = 0,
                time_fare = rate_per_minute * CEIL(EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - start_time)) / 60),
                total_fare = unlock_fee + (rate_per_minute * CEIL(EXTRACT(EPOCH FROM (CURRENT_TIMESTAMP - start_time)) / 60)),
                status = 'completed',
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @TripId AND status = 'active'
            RETURNING 
                id, user_id as UserId, vehicle_id as VehicleId,
                start_time as StartTime, end_time as EndTime,
                start_latitude as StartLatitude, start_longitude as StartLongitude,
                end_latitude as EndLatitude, end_longitude as EndLongitude,
                start_station_id as StartStationId, end_station_id as EndStationId,
                distance_km as DistanceKm, duration_minutes as DurationMinutes,
                unlock_fee as UnlockFee, rate_per_minute as RatePerMinute,
                base_fare as BaseFare, distance_fare as DistanceFare,
                time_fare as TimeFare, total_fare as TotalFare,
                status, cancellation_reason as CancellationReason,
                rating, review, created_at as CreatedAt, updated_at as UpdatedAt";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TripDataContract>(
            sql, 
            new { TripId = tripId, request.EndLatitude, request.EndLongitude, request.EndStationId }
        );
    }

    public async Task<bool> CancelAsync(Guid tripId, string reason)
    {
        const string sql = @"
            UPDATE trips
            SET 
                status = 'cancelled',
                cancellation_reason = @Reason,
                end_time = CURRENT_TIMESTAMP,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @TripId AND status = 'active'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { TripId = tripId, Reason = reason });
        return rows > 0;
    }

    public async Task<bool> AddLocationAsync(Guid tripId, decimal latitude, decimal longitude, int? speed, int? batteryLevel)
    {
        const string sql = @"
            INSERT INTO trip_locations (trip_id, latitude, longitude, speed, battery_level, timestamp)
            VALUES (@TripId, @Latitude, @Longitude, @Speed, @BatteryLevel, CURRENT_TIMESTAMP)";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(
            sql, 
            new { TripId = tripId, Latitude = latitude, Longitude = longitude, Speed = speed, BatteryLevel = batteryLevel }
        );
        return rows > 0;
    }

    public async Task<TripLocationDataContract[]> GetLocationsAsync(Guid tripId)
    {
        const string sql = @"
            SELECT 
                id, trip_id as TripId, latitude, longitude,
                speed, battery_level as BatteryLevel, timestamp
            FROM trip_locations
            WHERE trip_id = @TripId
            ORDER BY timestamp";

        using var connection = _connectionFactory.CreateConnection();
        var locations = await connection.QueryAsync<TripLocationDataContract>(sql, new { TripId = tripId });
        return locations.ToArray();
    }

    public async Task<bool> RateAsync(Guid tripId, int rating, string? review)
    {
        const string sql = @"
            UPDATE trips
            SET 
                rating = @Rating,
                review = @Review,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @TripId AND status = 'completed'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { TripId = tripId, Rating = rating, Review = review });
        return rows > 0;
    }

    public async Task<TripStatisticsDataContract> GetUserStatisticsAsync(Guid userId, DateTime? startDate, DateTime? endDate)
    {
        const string sql = @"
            SELECT 
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COALESCE(AVG(rating), 0) as AverageRating,
                COALESCE(MIN(start_time), CURRENT_TIMESTAMP) as PeriodStart,
                COALESCE(MAX(start_time), CURRENT_TIMESTAMP) as PeriodEnd
            FROM trips
            WHERE user_id = @UserId
              AND status IN ('completed', 'cancelled')
              AND (@StartDate IS NULL OR start_time >= @StartDate)
              AND (@EndDate IS NULL OR start_time <= @EndDate)";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<TripStatisticsDataContract>(
            sql, 
            new { UserId = userId, StartDate = startDate, EndDate = endDate }
        );
    }

    public async Task<TripStatisticsDataContract> GetVehicleStatisticsAsync(Guid vehicleId, DateTime? startDate, DateTime? endDate)
    {
        const string sql = @"
            SELECT 
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COALESCE(AVG(rating), 0) as AverageRating,
                COALESCE(MIN(start_time), CURRENT_TIMESTAMP) as PeriodStart,
                COALESCE(MAX(start_time), CURRENT_TIMESTAMP) as PeriodEnd
            FROM trips
            WHERE vehicle_id = @VehicleId
              AND status IN ('completed', 'cancelled')
              AND (@StartDate IS NULL OR start_time >= @StartDate)
              AND (@EndDate IS NULL OR start_time <= @EndDate)";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<TripStatisticsDataContract>(
            sql, 
            new { VehicleId = vehicleId, StartDate = startDate, EndDate = endDate }
        );
    }

    public async Task<TripStatisticsDataContract> GetSystemStatisticsAsync(DateTime? startDate, DateTime? endDate)
    {
        const string sql = @"
            SELECT 
                COUNT(*) as TotalTrips,
                COALESCE(SUM(distance_km), 0) as TotalDistanceKm,
                COALESCE(SUM(duration_minutes), 0) as TotalDurationMinutes,
                COALESCE(SUM(total_fare), 0) as TotalRevenue,
                COALESCE(AVG(rating), 0) as AverageRating,
                COALESCE(MIN(start_time), CURRENT_TIMESTAMP) as PeriodStart,
                COALESCE(MAX(start_time), CURRENT_TIMESTAMP) as PeriodEnd
            FROM trips
            WHERE status IN ('completed', 'cancelled')
              AND (@StartDate IS NULL OR start_time >= @StartDate)
              AND (@EndDate IS NULL OR start_time <= @EndDate)";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<TripStatisticsDataContract>(
            sql, 
            new { StartDate = startDate, EndDate = endDate }
        );
    }
}