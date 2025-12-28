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

namespace SmartCity.IoTService.Core.DTOs;

public class IoTDeviceDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public string? FirmwareVersion { get; set; }
    public DateTime? LastCommunication { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class IoTTelemetryDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid VehicleId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Speed { get; set; }
    public int? BatteryLevel { get; set; }
    public decimal? BatteryVoltage { get; set; }
    public int? Temperature { get; set; }
    public bool IsLocked { get; set; }
    public bool IsCharging { get; set; }
    public decimal? Odometer { get; set; }
    public string? ErrorCodes { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class IoTCommandDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid VehicleId { get; set; }
    public string CommandType { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Response { get; set; }
    public string? ErrorMessage { get; set; }
}

public class IoTAlertDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid VehicleId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
}

public class CreateDeviceRequest
{
    public Guid VehicleId { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string? FirmwareVersion { get; set; }
}

public class CreateTelemetryRequest
{
    public Guid DeviceId { get; set; }
    public Guid VehicleId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Speed { get; set; }
    public int? BatteryLevel { get; set; }
    public decimal? BatteryVoltage { get; set; }
    public int? Temperature { get; set; }
    public bool IsLocked { get; set; }
    public bool IsCharging { get; set; }
    public decimal? Odometer { get; set; }
    public string? ErrorCodes { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SendCommandRequest
{
    public string CommandType { get; set; } = string.Empty;
    public string? Parameters { get; set; }
}

public class CreateAlertRequest
{
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}

public class UpdateDeviceStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateFirmwareRequest
{
    public string FirmwareVersion { get; set; } = string.Empty;
}

public class ResolveAlertRequest
{
    public string ResolvedBy { get; set; } = string.Empty;
}