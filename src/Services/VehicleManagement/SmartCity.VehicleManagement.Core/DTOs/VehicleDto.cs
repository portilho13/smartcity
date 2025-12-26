namespace SmartCity.VehicleManagement.Core.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public Guid VehicleTypeId { get; set; }
    public string? LicensePlate { get; set; }
    public string QrCode { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string Status { get; set; } = "available";
    public int? BatteryLevel { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }
    public Guid? CurrentStationId { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public int TotalTrips { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StationType { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public int TotalCapacity { get; set; }
    public int AvailableSlots { get; set; }
    public int OccupiedSlots => TotalCapacity - AvailableSlots;
    public string Status { get; set; } = "operational";
    public bool HasCharging { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VehicleTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePricePerMinute { get; set; }
    public decimal UnlockFee { get; set; }
    public int? MaxSpeed { get; set; }
    public bool RequiresLicense { get; set; }
    public int MinAge { get; set; }
}

public class UpdateVehicleLocationRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int? BatteryLevel { get; set; }
}

public class UpdateVehicleStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class NearbyVehiclesRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int RadiusKm { get; set; } = 2;
}