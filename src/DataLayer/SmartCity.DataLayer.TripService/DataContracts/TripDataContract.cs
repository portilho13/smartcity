using System.Runtime.Serialization;

namespace SmartCity.DataLayer.TripService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public class TripDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid UserId { get; set; }

    [DataMember(Order = 3)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 4)]
    public DateTime StartTime { get; set; }

    [DataMember(Order = 5)]
    public DateTime? EndTime { get; set; }

    [DataMember(Order = 6)]
    public decimal? StartLatitude { get; set; }

    [DataMember(Order = 7)]
    public decimal? StartLongitude { get; set; }

    [DataMember(Order = 8)]
    public decimal? EndLatitude { get; set; }

    [DataMember(Order = 9)]
    public decimal? EndLongitude { get; set; }

    [DataMember(Order = 10)]
    public Guid? StartStationId { get; set; }

    [DataMember(Order = 11)]
    public Guid? EndStationId { get; set; }

    [DataMember(Order = 12)]
    public decimal? DistanceKm { get; set; }

    [DataMember(Order = 13)]
    public int? DurationMinutes { get; set; }

    [DataMember(Order = 14)]
    public decimal? BaseFare { get; set; }

    [DataMember(Order = 15)]
    public decimal? DistanceFare { get; set; }

    [DataMember(Order = 16)]
    public decimal? TimeFare { get; set; }

    [DataMember(Order = 17)]
    public decimal? TotalFare { get; set; }

    [DataMember(Order = 18)]
    public string Status { get; set; } = "active";

    [DataMember(Order = 19)]
    public string? CancellationReason { get; set; }

    [DataMember(Order = 20)]
    public int? Rating { get; set; }

    [DataMember(Order = 21)]
    public string? Review { get; set; }

    [DataMember(Order = 22)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 23)]
    public DateTime? UpdatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public class TripLocationDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid TripId { get; set; }

    [DataMember(Order = 3)]
    public decimal Latitude { get; set; }

    [DataMember(Order = 4)]
    public decimal Longitude { get; set; }

    [DataMember(Order = 5)]
    public int? Speed { get; set; }

    [DataMember(Order = 6)]
    public int? BatteryLevel { get; set; }

    [DataMember(Order = 7)]
    public DateTime Timestamp { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public class TripStatisticsDataContract
{
    [DataMember(Order = 1)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 2)]
    public decimal TotalDistanceKm { get; set; }

    [DataMember(Order = 3)]
    public int TotalDurationMinutes { get; set; }

    [DataMember(Order = 4)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 5)]
    public decimal AverageRating { get; set; }

    [DataMember(Order = 6)]
    public DateTime PeriodStart { get; set; }

    [DataMember(Order = 7)]
    public DateTime PeriodEnd { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public class CreateTripRequest
{
    [DataMember(Order = 1)]
    public Guid UserId { get; set; }

    [DataMember(Order = 2)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 3)]
    public decimal StartLatitude { get; set; }

    [DataMember(Order = 4)]
    public decimal StartLongitude { get; set; }

    [DataMember(Order = 5)]
    public Guid? StartStationId { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/tripdata/v1")]
public class EndTripRequest
{
    [DataMember(Order = 1)]
    public decimal EndLatitude { get; set; }

    [DataMember(Order = 2)]
    public decimal EndLongitude { get; set; }

    [DataMember(Order = 3)]
    public Guid? EndStationId { get; set; }
}