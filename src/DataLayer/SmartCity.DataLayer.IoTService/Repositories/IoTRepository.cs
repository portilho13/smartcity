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
using SmartCity.DataLayer.IoTService.DataContracts;
using SmartCity.DataLayer.IoTService.Infrastructure;
using System.Data;

namespace SmartCity.DataLayer.IoTService.Repositories;

public class IoTRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<IoTRepository> _logger;

    public IoTRepository(DatabaseConnectionFactory connectionFactory, ILogger<IoTRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // ==================== Device Operations ====================

    public async Task<IoTDeviceDataContract?> GetDeviceByIdAsync(Guid deviceId)
    {
        const string sql = @"
            SELECT 
                id, vehicle_id as VehicleId, device_identifier as DeviceIdentifier,
                device_type as DeviceType, status, firmware_version as FirmwareVersion,
                last_communication as LastCommunication, created_at as CreatedAt,
                updated_at as UpdatedAt
            FROM iot_devices
            WHERE id = @DeviceId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTDeviceDataContract>(sql, new { DeviceId = deviceId });
    }

    public async Task<IoTDeviceDataContract?> GetDeviceByVehicleIdAsync(Guid vehicleId)
    {
        const string sql = @"
            SELECT 
                id, vehicle_id as VehicleId, device_identifier as DeviceIdentifier,
                device_type as DeviceType, status, firmware_version as FirmwareVersion,
                last_communication as LastCommunication, created_at as CreatedAt,
                updated_at as UpdatedAt
            FROM iot_devices
            WHERE vehicle_id = @VehicleId
            LIMIT 1";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTDeviceDataContract>(sql, new { VehicleId = vehicleId });
    }

    public async Task<IoTDeviceDataContract?> GetDeviceByIdentifierAsync(string deviceIdentifier)
    {
        const string sql = @"
            SELECT 
                id, vehicle_id as VehicleId, device_identifier as DeviceIdentifier,
                device_type as DeviceType, status, firmware_version as FirmwareVersion,
                last_communication as LastCommunication, created_at as CreatedAt,
                updated_at as UpdatedAt
            FROM iot_devices
            WHERE device_identifier = @DeviceIdentifier";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTDeviceDataContract>(sql, new { DeviceIdentifier = deviceIdentifier });
    }

    public async Task<IoTDeviceDataContract[]> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, vehicle_id as VehicleId, device_identifier as DeviceIdentifier,
                device_type as DeviceType, status, firmware_version as FirmwareVersion,
                last_communication as LastCommunication, created_at as CreatedAt,
                updated_at as UpdatedAt
            FROM iot_devices
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var devices = await connection.QueryAsync<IoTDeviceDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return devices.ToArray();
    }

    public async Task<IoTDeviceDataContract> CreateAsync(CreateIoTDeviceRequest request)
    {
        const string sql = @"
            INSERT INTO iot_devices (
                vehicle_id, device_identifier, device_type, 
                status, firmware_version, created_at
            )
            VALUES (
                @VehicleId, @DeviceIdentifier, @DeviceType,
                'active', @FirmwareVersion, CURRENT_TIMESTAMP
            )
            RETURNING 
                id, vehicle_id as VehicleId, device_identifier as DeviceIdentifier,
                device_type as DeviceType, status, firmware_version as FirmwareVersion,
                last_communication as LastCommunication, created_at as CreatedAt,
                updated_at as UpdatedAt";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<IoTDeviceDataContract>(sql, request);
    }

    public async Task<bool> UpdateStatusAsync(Guid deviceId, string status)
    {
        const string sql = @"
            UPDATE iot_devices
            SET 
                status = @Status,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @DeviceId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { DeviceId = deviceId, Status = status });
        return rows > 0;
    }

    public async Task<bool> UpdateFirmwareAsync(Guid deviceId, string firmwareVersion)
    {
        const string sql = @"
            UPDATE iot_devices
            SET 
                firmware_version = @FirmwareVersion,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @DeviceId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { DeviceId = deviceId, FirmwareVersion = firmwareVersion });
        return rows > 0;
    }

    // ==================== Telemetry Operations ====================

    public async Task<IoTTelemetryDataContract?> GetLatestTelemetryAsync(Guid vehicleId)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                latitude, longitude, speed, battery_level as BatteryLevel,
                battery_voltage as BatteryVoltage, temperature,
                is_locked as IsLocked, is_charging as IsCharging,
                odometer, error_codes as ErrorCodes,
                timestamp, created_at as CreatedAt
            FROM iot_telemetry
            WHERE vehicle_id = @VehicleId
            ORDER BY timestamp DESC
            LIMIT 1";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTTelemetryDataContract>(sql, new { VehicleId = vehicleId });
    }

    public async Task<IoTTelemetryDataContract[]> GetTelemetryByVehicleAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                latitude, longitude, speed, battery_level as BatteryLevel,
                battery_voltage as BatteryVoltage, temperature,
                is_locked as IsLocked, is_charging as IsCharging,
                odometer, error_codes as ErrorCodes,
                timestamp, created_at as CreatedAt
            FROM iot_telemetry
            WHERE vehicle_id = @VehicleId
              AND timestamp >= @StartDate
              AND timestamp <= @EndDate
            ORDER BY timestamp DESC";

        using var connection = _connectionFactory.CreateConnection();
        var telemetry = await connection.QueryAsync<IoTTelemetryDataContract>(
            sql, 
            new { VehicleId = vehicleId, StartDate = startDate, EndDate = endDate }
        );
        return telemetry.ToArray();
    }

    public async Task<IoTTelemetryDataContract[]> GetTelemetryByDeviceAsync(Guid deviceId, DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                latitude, longitude, speed, battery_level as BatteryLevel,
                battery_voltage as BatteryVoltage, temperature,
                is_locked as IsLocked, is_charging as IsCharging,
                odometer, error_codes as ErrorCodes,
                timestamp, created_at as CreatedAt
            FROM iot_telemetry
            WHERE device_id = @DeviceId
              AND timestamp >= @StartDate
              AND timestamp <= @EndDate
            ORDER BY timestamp DESC";

        using var connection = _connectionFactory.CreateConnection();
        var telemetry = await connection.QueryAsync<IoTTelemetryDataContract>(
            sql, 
            new { DeviceId = deviceId, StartDate = startDate, EndDate = endDate }
        );
        return telemetry.ToArray();
    }

    public async Task<IoTTelemetryDataContract> CreateTelemetryAsync(CreateTelemetryRequest request)
    {
        const string sql = @"
            INSERT INTO iot_telemetry (
                device_id, vehicle_id, latitude, longitude, speed,
                battery_level, battery_voltage, temperature,
                is_locked, is_charging, odometer, error_codes,
                timestamp, created_at
            )
            VALUES (
                @DeviceId, @VehicleId, @Latitude, @Longitude, @Speed,
                @BatteryLevel, @BatteryVoltage, @Temperature,
                @IsLocked, @IsCharging, @Odometer, @ErrorCodes,
                @Timestamp, CURRENT_TIMESTAMP
            )
            RETURNING 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                latitude, longitude, speed, battery_level as BatteryLevel,
                battery_voltage as BatteryVoltage, temperature,
                is_locked as IsLocked, is_charging as IsCharging,
                odometer, error_codes as ErrorCodes,
                timestamp, created_at as CreatedAt";

        using var connection = _connectionFactory.CreateConnection();
        var telemetry = await connection.QuerySingleAsync<IoTTelemetryDataContract>(sql, request);

        // Update device last communication
        await UpdateDeviceLastCommunicationAsync(connection, request.DeviceId);

        return telemetry;
    }

    private async Task UpdateDeviceLastCommunicationAsync(IDbConnection connection, Guid deviceId)
    {
        const string sql = @"
            UPDATE iot_devices
            SET last_communication = CURRENT_TIMESTAMP
            WHERE id = @DeviceId";

        await connection.ExecuteAsync(sql, new { DeviceId = deviceId });
    }

    // ==================== Command Operations ====================

    public async Task<IoTCommandDataContract?> GetCommandByIdAsync(Guid commandId)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                command_type as CommandType, parameters, status,
                created_at as CreatedAt, sent_at as SentAt,
                acknowledged_at as AcknowledgedAt, completed_at as CompletedAt,
                response, error_message as ErrorMessage
            FROM iot_commands
            WHERE id = @CommandId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTCommandDataContract>(sql, new { CommandId = commandId });
    }

    public async Task<IoTCommandDataContract[]> GetCommandsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                command_type as CommandType, parameters, status,
                created_at as CreatedAt, sent_at as SentAt,
                acknowledged_at as AcknowledgedAt, completed_at as CompletedAt,
                response, error_message as ErrorMessage
            FROM iot_commands
            WHERE vehicle_id = @VehicleId
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var commands = await connection.QueryAsync<IoTCommandDataContract>(sql, new { VehicleId = vehicleId, PageSize = pageSize, Offset = offset });
        return commands.ToArray();
    }

    public async Task<IoTCommandDataContract[]> GetPendingCommandsAsync(Guid deviceId)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                command_type as CommandType, parameters, status,
                created_at as CreatedAt, sent_at as SentAt,
                acknowledged_at as AcknowledgedAt, completed_at as CompletedAt,
                response, error_message as ErrorMessage
            FROM iot_commands
            WHERE device_id = @DeviceId
              AND status IN ('pending', 'sent')
            ORDER BY created_at";

        using var connection = _connectionFactory.CreateConnection();
        var commands = await connection.QueryAsync<IoTCommandDataContract>(sql, new { DeviceId = deviceId });
        return commands.ToArray();
    }

    public async Task<IoTCommandDataContract> SendCommandAsync(SendCommandRequest request)
    {
        const string sql = @"
            INSERT INTO iot_commands (
                device_id, vehicle_id, command_type, parameters,
                status, created_at, sent_at
            )
            VALUES (
                @DeviceId, @VehicleId, @CommandType, @Parameters,
                'sent', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
            )
            RETURNING 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                command_type as CommandType, parameters, status,
                created_at as CreatedAt, sent_at as SentAt,
                acknowledged_at as AcknowledgedAt, completed_at as CompletedAt,
                response, error_message as ErrorMessage";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<IoTCommandDataContract>(sql, request);
    }

    public async Task<bool> UpdateCommandStatusAsync(Guid commandId, string status, string? response, string? errorMessage)
    {
        const string sql = @"
            UPDATE iot_commands
            SET 
                status = @Status,
                response = @Response,
                error_message = @ErrorMessage,
                acknowledged_at = CASE WHEN @Status = 'acknowledged' THEN CURRENT_TIMESTAMP ELSE acknowledged_at END,
                completed_at = CASE WHEN @Status IN ('completed', 'failed') THEN CURRENT_TIMESTAMP ELSE completed_at END
            WHERE id = @CommandId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { CommandId = commandId, Status = status, Response = response, ErrorMessage = errorMessage });
        return rows > 0;
    }

    // ==================== Alert Operations ====================

    public async Task<IoTAlertDataContract?> GetAlertByIdAsync(Guid alertId)
    {
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                alert_type as AlertType, severity, message, details, status,
                created_at as CreatedAt, resolved_at as ResolvedAt,
                resolved_by as ResolvedBy
            FROM iot_alerts
            WHERE id = @AlertId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<IoTAlertDataContract>(sql, new { AlertId = alertId });
    }

    public async Task<IoTAlertDataContract[]> GetAlertsByVehicleAsync(Guid vehicleId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                alert_type as AlertType, severity, message, details, status,
                created_at as CreatedAt, resolved_at as ResolvedAt,
                resolved_by as ResolvedBy
            FROM iot_alerts
            WHERE vehicle_id = @VehicleId
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var alerts = await connection.QueryAsync<IoTAlertDataContract>(sql, new { VehicleId = vehicleId, PageSize = pageSize, Offset = offset });
        return alerts.ToArray();
    }

    public async Task<IoTAlertDataContract[]> GetActiveAlertsAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                alert_type as AlertType, severity, message, details, status,
                created_at as CreatedAt, resolved_at as ResolvedAt,
                resolved_by as ResolvedBy
            FROM iot_alerts
            WHERE status = 'active'
            ORDER BY severity DESC, created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var alerts = await connection.QueryAsync<IoTAlertDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return alerts.ToArray();
    }

    public async Task<IoTAlertDataContract> CreateAlertAsync(CreateAlertRequest request)
    {
        const string sql = @"
            INSERT INTO iot_alerts (
                device_id, vehicle_id, alert_type, severity,
                message, details, status, created_at
            )
            VALUES (
                @DeviceId, @VehicleId, @AlertType, @Severity,
                @Message, @Details, 'active', CURRENT_TIMESTAMP
            )
            RETURNING 
                id, device_id as DeviceId, vehicle_id as VehicleId,
                alert_type as AlertType, severity, message, details, status,
                created_at as CreatedAt, resolved_at as ResolvedAt,
                resolved_by as ResolvedBy";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<IoTAlertDataContract>(sql, request);
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId, string resolvedBy)
    {
        const string sql = @"
            UPDATE iot_alerts
            SET 
                status = 'resolved',
                resolved_at = CURRENT_TIMESTAMP,
                resolved_by = @ResolvedBy
            WHERE id = @AlertId AND status = 'active'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { AlertId = alertId, ResolvedBy = resolvedBy });
        return rows > 0;
    }
}