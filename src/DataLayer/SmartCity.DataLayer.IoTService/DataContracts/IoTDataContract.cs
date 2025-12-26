using System.Runtime.Serialization;

namespace SmartCity.DataLayer.IoTService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class IoTDeviceDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 3)]
    public string DeviceIdentifier { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string DeviceType { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string Status { get; set; } = "active";

    [DataMember(Order = 6)]
    public string? FirmwareVersion { get; set; }

    [DataMember(Order = 7)]
    public DateTime? LastCommunication { get; set; }

    [DataMember(Order = 8)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 9)]
    public DateTime? UpdatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class IoTTelemetryDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 3)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 4)]
    public decimal? Latitude { get; set; }

    [DataMember(Order = 5)]
    public decimal? Longitude { get; set; }

    [DataMember(Order = 6)]
    public int? Speed { get; set; }

    [DataMember(Order = 7)]
    public int? BatteryLevel { get; set; }

    [DataMember(Order = 8)]
    public decimal? BatteryVoltage { get; set; }

    [DataMember(Order = 9)]
    public int? Temperature { get; set; }

    [DataMember(Order = 10)]
    public bool IsLocked { get; set; }

    [DataMember(Order = 11)]
    public bool IsCharging { get; set; }

    [DataMember(Order = 12)]
    public decimal? Odometer { get; set; }

    [DataMember(Order = 13)]
    public string? ErrorCodes { get; set; }

    [DataMember(Order = 14)]
    public DateTime Timestamp { get; set; }

    [DataMember(Order = 15)]
    public DateTime CreatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class IoTCommandDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 3)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 4)]
    public string CommandType { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string? Parameters { get; set; }

    [DataMember(Order = 6)]
    public string Status { get; set; } = "pending";

    [DataMember(Order = 7)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 8)]
    public DateTime? SentAt { get; set; }

    [DataMember(Order = 9)]
    public DateTime? AcknowledgedAt { get; set; }

    [DataMember(Order = 10)]
    public DateTime? CompletedAt { get; set; }

    [DataMember(Order = 11)]
    public string? Response { get; set; }

    [DataMember(Order = 12)]
    public string? ErrorMessage { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class IoTAlertDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 3)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 4)]
    public string AlertType { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string Severity { get; set; } = "info";

    [DataMember(Order = 6)]
    public string Message { get; set; } = string.Empty;

    [DataMember(Order = 7)]
    public string? Details { get; set; }

    [DataMember(Order = 8)]
    public string Status { get; set; } = "active";

    [DataMember(Order = 9)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 10)]
    public DateTime? ResolvedAt { get; set; }

    [DataMember(Order = 11)]
    public string? ResolvedBy { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class CreateIoTDeviceRequest
{
    [DataMember(Order = 1)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 2)]
    public string DeviceIdentifier { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string DeviceType { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string? FirmwareVersion { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class CreateTelemetryRequest
{
    [DataMember(Order = 1)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 3)]
    public decimal? Latitude { get; set; }

    [DataMember(Order = 4)]
    public decimal? Longitude { get; set; }

    [DataMember(Order = 5)]
    public int? Speed { get; set; }

    [DataMember(Order = 6)]
    public int? BatteryLevel { get; set; }

    [DataMember(Order = 7)]
    public decimal? BatteryVoltage { get; set; }

    [DataMember(Order = 8)]
    public int? Temperature { get; set; }

    [DataMember(Order = 9)]
    public bool IsLocked { get; set; }

    [DataMember(Order = 10)]
    public bool IsCharging { get; set; }

    [DataMember(Order = 11)]
    public decimal? Odometer { get; set; }

    [DataMember(Order = 12)]
    public string? ErrorCodes { get; set; }

    [DataMember(Order = 13)]
    public DateTime Timestamp { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class SendCommandRequest
{
    [DataMember(Order = 1)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 3)]
    public string CommandType { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string? Parameters { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/iotdata/v1")]
public class CreateAlertRequest
{
    [DataMember(Order = 1)]
    public Guid DeviceId { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 3)]
    public string AlertType { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string Severity { get; set; } = "info";

    [DataMember(Order = 5)]
    public string Message { get; set; } = string.Empty;

    [DataMember(Order = 6)]
    public string? Details { get; set; }
}