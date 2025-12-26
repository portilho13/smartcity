using System.Runtime.Serialization;

namespace SmartCity.DataLayer.VehicleService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/vehicledata/v1")]
public class VehicleDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleTypeId { get; set; }

    [DataMember(Order = 3)]
    public string? LicensePlate { get; set; }

    [DataMember(Order = 4)]
    public string QrCode { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string? Model { get; set; }

    [DataMember(Order = 6)]
    public string? Manufacturer { get; set; }

    [DataMember(Order = 7)]
    public int? Year { get; set; }

    [DataMember(Order = 8)]
    public string? Color { get; set; }

    [DataMember(Order = 9)]
    public string Status { get; set; } = "available";

    [DataMember(Order = 10)]
    public int? BatteryLevel { get; set; }

    [DataMember(Order = 11)]
    public decimal? CurrentLatitude { get; set; }

    [DataMember(Order = 12)]
    public decimal? CurrentLongitude { get; set; }

    [DataMember(Order = 13)]
    public DateTime? LastLocationUpdate { get; set; }

    [DataMember(Order = 14)]
    public Guid? CurrentStationId { get; set; }

    [DataMember(Order = 15)]
    public decimal TotalDistanceKm { get; set; }

    [DataMember(Order = 16)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 17)]
    public DateTime CreatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/vehicledata/v1")]
public class StationDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string StationType { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public decimal Latitude { get; set; }

    [DataMember(Order = 5)]
    public decimal Longitude { get; set; }

    [DataMember(Order = 6)]
    public string? Address { get; set; }

    [DataMember(Order = 7)]
    public string? City { get; set; }

    [DataMember(Order = 8)]
    public int TotalCapacity { get; set; }

    [DataMember(Order = 9)]
    public int AvailableSlots { get; set; }

    [DataMember(Order = 10)]
    public string Status { get; set; } = "operational";

    [DataMember(Order = 11)]
    public bool HasCharging { get; set; }

    [DataMember(Order = 12)]
    public DateTime CreatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/vehicledata/v1")]
public class VehicleTypeDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string? Description { get; set; }

    [DataMember(Order = 4)]
    public decimal BasePricePerMinute { get; set; }

    [DataMember(Order = 5)]
    public decimal UnlockFee { get; set; }

    [DataMember(Order = 6)]
    public int? MaxSpeed { get; set; }

    [DataMember(Order = 7)]
    public bool RequiresLicense { get; set; }

    [DataMember(Order = 8)]
    public int MinAge { get; set; }
}