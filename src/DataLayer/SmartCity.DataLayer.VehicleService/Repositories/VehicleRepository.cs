using Dapper;
using SmartCity.DataLayer.VehicleService.DataContracts;
using SmartCity.DataLayer.VehicleService.Infrastructure;

namespace SmartCity.DataLayer.VehicleService.Repositories;

public class VehicleRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<VehicleRepository> _logger;

    public VehicleRepository(DatabaseConnectionFactory connectionFactory, ILogger<VehicleRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<VehicleDataContract?> GetByIdAsync(Guid vehicleId)
    {
        const string sql = @"
            SELECT 
                id, vehicle_type_id as VehicleTypeId, license_plate as LicensePlate,
                qr_code as QrCode, model, manufacturer, year, color, status,
                battery_level as BatteryLevel, current_latitude as CurrentLatitude,
                current_longitude as CurrentLongitude, last_location_update as LastLocationUpdate,
                current_station_id as CurrentStationId, total_distance_km as TotalDistanceKm,
                total_trips as TotalTrips, created_at as CreatedAt
            FROM vehicles
            WHERE id = @VehicleId AND retired_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<VehicleDataContract>(sql, new { VehicleId = vehicleId });
    }

    public async Task<VehicleDataContract?> GetByQrCodeAsync(string qrCode)
    {
        const string sql = @"
            SELECT 
                id, vehicle_type_id as VehicleTypeId, license_plate as LicensePlate,
                qr_code as QrCode, model, manufacturer, year, color, status,
                battery_level as BatteryLevel, current_latitude as CurrentLatitude,
                current_longitude as CurrentLongitude, last_location_update as LastLocationUpdate,
                current_station_id as CurrentStationId, total_distance_km as TotalDistanceKm,
                total_trips as TotalTrips, created_at as CreatedAt
            FROM vehicles
            WHERE qr_code = @QrCode AND retired_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<VehicleDataContract>(sql, new { QrCode = qrCode });
    }

    public async Task<VehicleDataContract[]> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, vehicle_type_id as VehicleTypeId, license_plate as LicensePlate,
                qr_code as QrCode, model, manufacturer, year, color, status,
                battery_level as BatteryLevel, current_latitude as CurrentLatitude,
                current_longitude as CurrentLongitude, last_location_update as LastLocationUpdate,
                current_station_id as CurrentStationId, total_distance_km as TotalDistanceKm,
                total_trips as TotalTrips, created_at as CreatedAt
            FROM vehicles
            WHERE retired_at IS NULL
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var vehicles = await connection.QueryAsync<VehicleDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return vehicles.ToArray();
    }

    public async Task<VehicleDataContract[]> GetAvailableAsync()
    {
        const string sql = @"
            SELECT 
                id, vehicle_type_id as VehicleTypeId, license_plate as LicensePlate,
                qr_code as QrCode, model, manufacturer, year, color, status,
                battery_level as BatteryLevel, current_latitude as CurrentLatitude,
                current_longitude as CurrentLongitude, last_location_update as LastLocationUpdate,
                current_station_id as CurrentStationId, total_distance_km as TotalDistanceKm,
                total_trips as TotalTrips, created_at as CreatedAt
            FROM vehicles
            WHERE status = 'available' AND retired_at IS NULL
            ORDER BY battery_level DESC NULLS LAST";

        using var connection = _connectionFactory.CreateConnection();
        var vehicles = await connection.QueryAsync<VehicleDataContract>(sql);
        return vehicles.ToArray();
    }

    public async Task<VehicleDataContract[]> GetNearbyAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        const string sql = @"
            SELECT 
                id, vehicle_type_id as VehicleTypeId, license_plate as LicensePlate,
                qr_code as QrCode, model, manufacturer, year, color, status,
                battery_level as BatteryLevel, current_latitude as CurrentLatitude,
                current_longitude as CurrentLongitude, last_location_update as LastLocationUpdate,
                current_station_id as CurrentStationId, total_distance_km as TotalDistanceKm,
                total_trips as TotalTrips, created_at as CreatedAt
            FROM vehicles
            WHERE status = 'available' 
              AND retired_at IS NULL
              AND current_latitude IS NOT NULL 
              AND current_longitude IS NOT NULL
              AND earth_distance(
                    ll_to_earth(@Latitude, @Longitude),
                    ll_to_earth(current_latitude, current_longitude)
                  ) <= @RadiusMeters
            ORDER BY earth_distance(
                       ll_to_earth(@Latitude, @Longitude),
                       ll_to_earth(current_latitude, current_longitude)
                     )";

        using var connection = _connectionFactory.CreateConnection();
        var vehicles = await connection.QueryAsync<VehicleDataContract>(
            sql, 
            new { Latitude = latitude, Longitude = longitude, RadiusMeters = radiusKm * 1000 }
        );
        return vehicles.ToArray();
    }

    public async Task<bool> UpdateLocationAsync(Guid vehicleId, decimal latitude, decimal longitude, int? batteryLevel)
    {
        const string sql = @"
            UPDATE vehicles
            SET 
                current_latitude = @Latitude,
                current_longitude = @Longitude,
                battery_level = COALESCE(@BatteryLevel, battery_level),
                last_location_update = CURRENT_TIMESTAMP,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @VehicleId AND retired_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { VehicleId = vehicleId, Latitude = latitude, Longitude = longitude, BatteryLevel = batteryLevel });
        return rows > 0;
    }

    public async Task<bool> UpdateStatusAsync(Guid vehicleId, string status)
    {
        const string sql = @"
            UPDATE vehicles
            SET 
                status = @Status,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @VehicleId AND retired_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { VehicleId = vehicleId, Status = status });
        return rows > 0;
    }

    // Stations
    public async Task<StationDataContract?> GetStationByIdAsync(Guid stationId)
    {
        const string sql = @"
            SELECT 
                id, name, station_type as StationType, latitude, longitude,
                address, city, total_capacity as TotalCapacity, 
                available_slots as AvailableSlots, status, 
                has_charging as HasCharging, created_at as CreatedAt
            FROM stations
            WHERE id = @StationId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<StationDataContract>(sql, new { StationId = stationId });
    }

    public async Task<StationDataContract[]> GetAllStationsAsync()
    {
        const string sql = @"
            SELECT 
                id, name, station_type as StationType, latitude, longitude,
                address, city, total_capacity as TotalCapacity, 
                available_slots as AvailableSlots, status, 
                has_charging as HasCharging, created_at as CreatedAt
            FROM stations
            ORDER BY name";

        using var connection = _connectionFactory.CreateConnection();
        var stations = await connection.QueryAsync<StationDataContract>(sql);
        return stations.ToArray();
    }

    public async Task<StationDataContract[]> GetNearbyStationsAsync(decimal latitude, decimal longitude, int radiusKm)
    {
        const string sql = @"
            SELECT 
                id, name, station_type as StationType, latitude, longitude,
                address, city, total_capacity as TotalCapacity, 
                available_slots as AvailableSlots, status, 
                has_charging as HasCharging, created_at as CreatedAt
            FROM stations
            WHERE status = 'operational'
              AND earth_distance(
                    ll_to_earth(@Latitude, @Longitude),
                    ll_to_earth(latitude, longitude)
                  ) <= @RadiusMeters
            ORDER BY earth_distance(
                       ll_to_earth(@Latitude, @Longitude),
                       ll_to_earth(latitude, longitude)
                     )";

        using var connection = _connectionFactory.CreateConnection();
        var stations = await connection.QueryAsync<StationDataContract>(
            sql, 
            new { Latitude = latitude, Longitude = longitude, RadiusMeters = radiusKm * 1000 }
        );
        return stations.ToArray();
    }

    // Vehicle Types
    public async Task<VehicleTypeDataContract[]> GetAllVehicleTypesAsync()
    {
        const string sql = @"
            SELECT 
                id, name, description,
                base_price_per_minute as BasePricePerMinute,
                unlock_fee as UnlockFee, max_speed as MaxSpeed,
                requires_license as RequiresLicense, min_age as MinAge
            FROM vehicle_types
            ORDER BY name";

        using var connection = _connectionFactory.CreateConnection();
        var types = await connection.QueryAsync<VehicleTypeDataContract>(sql);
        return types.ToArray();
    }
}