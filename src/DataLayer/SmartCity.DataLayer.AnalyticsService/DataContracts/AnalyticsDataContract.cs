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

using System.Runtime.Serialization;

namespace SmartCity.DataLayer.AnalyticsService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class VehicleAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public Guid VehicleId { get; set; }

    [DataMember(Order = 2)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 3)]
    public decimal TotalDistanceKm { get; set; }

    [DataMember(Order = 4)]
    public int TotalDurationMinutes { get; set; }

    [DataMember(Order = 5)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 6)]
    public decimal AverageRating { get; set; }

    [DataMember(Order = 7)]
    public int TotalRatings { get; set; }

    [DataMember(Order = 8)]
    public decimal UtilizationRate { get; set; }

    [DataMember(Order = 9)]
    public DateTime PeriodStart { get; set; }

    [DataMember(Order = 10)]
    public DateTime PeriodEnd { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class UserAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public Guid UserId { get; set; }

    [DataMember(Order = 2)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 3)]
    public decimal TotalDistanceKm { get; set; }

    [DataMember(Order = 4)]
    public int TotalDurationMinutes { get; set; }

    [DataMember(Order = 5)]
    public decimal TotalSpent { get; set; }

    [DataMember(Order = 6)]
    public decimal AverageRating { get; set; }

    [DataMember(Order = 7)]
    public DateTime? LastTripDate { get; set; }

    [DataMember(Order = 8)]
    public string? FavoriteVehicleType { get; set; }

    [DataMember(Order = 9)]
    public DateTime PeriodStart { get; set; }

    [DataMember(Order = 10)]
    public DateTime PeriodEnd { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class SystemAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public int TotalUsers { get; set; }

    [DataMember(Order = 2)]
    public int ActiveUsers { get; set; }

    [DataMember(Order = 3)]
    public int TotalVehicles { get; set; }

    [DataMember(Order = 4)]
    public int AvailableVehicles { get; set; }

    [DataMember(Order = 5)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 6)]
    public int ActiveTrips { get; set; }

    [DataMember(Order = 7)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 8)]
    public decimal TotalDistanceKm { get; set; }

    [DataMember(Order = 9)]
    public decimal AverageRating { get; set; }

    [DataMember(Order = 10)]
    public decimal AverageTripDuration { get; set; }

    [DataMember(Order = 11)]
    public DateTime PeriodStart { get; set; }

    [DataMember(Order = 12)]
    public DateTime PeriodEnd { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class RevenueAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public DateTime Date { get; set; }

    [DataMember(Order = 2)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 3)]
    public int TripCount { get; set; }

    [DataMember(Order = 4)]
    public decimal AverageRevenuePerTrip { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class PopularRouteDataContract
{
    [DataMember(Order = 1)]
    public string RouteName { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public decimal StartLatitude { get; set; }

    [DataMember(Order = 3)]
    public decimal StartLongitude { get; set; }

    [DataMember(Order = 4)]
    public decimal EndLatitude { get; set; }

    [DataMember(Order = 5)]
    public decimal EndLongitude { get; set; }

    [DataMember(Order = 6)]
    public int TripCount { get; set; }

    [DataMember(Order = 7)]
    public decimal AverageDistance { get; set; }

    [DataMember(Order = 8)]
    public decimal AverageDuration { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class VehicleTypeAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public Guid VehicleTypeId { get; set; }

    [DataMember(Order = 2)]
    public string VehicleTypeName { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public int TotalVehicles { get; set; }

    [DataMember(Order = 4)]
    public int TotalTrips { get; set; }

    [DataMember(Order = 5)]
    public decimal TotalRevenue { get; set; }

    [DataMember(Order = 6)]
    public decimal AverageRating { get; set; }

    [DataMember(Order = 7)]
    public decimal UtilizationRate { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class HourlyUsageDataContract
{
    [DataMember(Order = 1)]
    public int Hour { get; set; }

    [DataMember(Order = 2)]
    public int TripCount { get; set; }

    [DataMember(Order = 3)]
    public decimal Revenue { get; set; }

    [DataMember(Order = 4)]
    public int UniqueUsers { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/analyticsdata/v1")]
public class StationAnalyticsDataContract
{
    [DataMember(Order = 1)]
    public Guid StationId { get; set; }

    [DataMember(Order = 2)]
    public string StationName { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public int TripsStarted { get; set; }

    [DataMember(Order = 4)]
    public int TripsEnded { get; set; }

    [DataMember(Order = 5)]
    public decimal AverageOccupancy { get; set; }

    [DataMember(Order = 6)]
    public DateTime PeriodStart { get; set; }

    [DataMember(Order = 7)]
    public DateTime PeriodEnd { get; set; }
}