namespace SmartCity.AnalyticsService.Core.DTOs;

public class VehicleAnalyticsDto
{
    public Guid VehicleId { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public int TotalDurationMinutes { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public decimal UtilizationRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class UserAnalyticsDto
{
    public Guid UserId { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public int TotalDurationMinutes { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime? LastTripDate { get; set; }
    public string? FavoriteVehicleType { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class SystemAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int TotalTrips { get; set; }
    public int ActiveTrips { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public decimal AverageRating { get; set; }
    public decimal AverageTripDuration { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class RevenueAnalyticsDto
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TripCount { get; set; }
    public decimal AverageRevenuePerTrip { get; set; }
}

public class PopularRouteDto
{
    public string RouteName { get; set; } = string.Empty;
    public decimal StartLatitude { get; set; }
    public decimal StartLongitude { get; set; }
    public decimal EndLatitude { get; set; }
    public decimal EndLongitude { get; set; }
    public int TripCount { get; set; }
    public decimal AverageDistance { get; set; }
    public decimal AverageDuration { get; set; }
}

public class VehicleTypeAnalyticsDto
{
    public Guid VehicleTypeId { get; set; }
    public string VehicleTypeName { get; set; } = string.Empty;
    public int TotalVehicles { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public decimal UtilizationRate { get; set; }
}

public class HourlyUsageDto
{
    public int Hour { get; set; }
    public int TripCount { get; set; }
    public decimal Revenue { get; set; }
    public int UniqueUsers { get; set; }
}

public class StationAnalyticsDto
{
    public Guid StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public int TripsStarted { get; set; }
    public int TripsEnded { get; set; }
    public decimal AverageOccupancy { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}