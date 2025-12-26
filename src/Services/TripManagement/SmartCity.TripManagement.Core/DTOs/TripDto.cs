namespace SmartCity.TripManagement.Core.DTOs;

public class TripDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? StartLatitude { get; set; }
    public decimal? StartLongitude { get; set; }
    public decimal? EndLatitude { get; set; }
    public decimal? EndLongitude { get; set; }
    public Guid? StartStationId { get; set; }
    public Guid? EndStationId { get; set; }
    public decimal? DistanceKm { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? BaseFare { get; set; }
    public decimal? DistanceFare { get; set; }
    public decimal? TimeFare { get; set; }
    public decimal? TotalFare { get; set; }
    public string Status { get; set; } = "active";
    public string? CancellationReason { get; set; }
    public int? Rating { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TripLocationDto
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int? Speed { get; set; }
    public int? BatteryLevel { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TripStatisticsDto
{
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public int TotalDurationMinutes { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class StartTripRequest
{
    public Guid VehicleId { get; set; }
    public decimal StartLatitude { get; set; }
    public decimal StartLongitude { get; set; }
    public Guid? StartStationId { get; set; }
}

public class EndTripRequest
{
    public decimal EndLatitude { get; set; }
    public decimal EndLongitude { get; set; }
    public Guid? EndStationId { get; set; }
}

public class CancelTripRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AddTripLocationRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int? Speed { get; set; }
    public int? BatteryLevel { get; set; }
}

public class RateTripRequest
{
    public int Rating { get; set; }
    public string? Review { get; set; }
}