using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.TripManagement.Core.DTOs;
using SmartCity.TripManagement.Core.Services;
using System.Security.Claims;
using SmartCity.IoTService.Core.DTOs;
using SmartCity.PaymentService.Core.DTOs;
using SmartCity.VehicleManagement.Core.DTOs;

namespace SmartCity.TripManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TripsController> _logger;

    public TripsController(
        IHttpClientFactory httpClientFactory, 
        ITripService tripService, 
        ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");
        
        return userId;
    }

    private HttpClient GetAuthenticatedHttpClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
    
        // Get the full Authorization header value
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
    
        _logger.LogInformation("Raw Authorization header: '{Header}'", authHeader);
    
        if (!string.IsNullOrEmpty(authHeader))
        {
            // Check if it starts with "Bearer "
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Extract just the token part
                var token = authHeader.Substring(7); // "Bearer " is 7 characters
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
                _logger.LogInformation("Token extracted and set: {Token}", token);
            }
            else
            {
                // If it doesn't have "Bearer ", assume it's just the token
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authHeader);
            
                _logger.LogInformation("Token set directly: {Token}", authHeader);
            }
        }
        else
        {
            _logger.LogWarning("No Authorization header found in request");
        }
    
        return httpClient;
    }

    /// <summary>
    /// Get all trips (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var trips = await _tripService.GetAllTripsAsync(page, pageSize);
        return Ok(trips);
    }

    /// <summary>
    /// Get current user's trips
    /// </summary>
    [HttpGet("my-trips")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        var trips = await _tripService.GetTripsByUserAsync(userId, page, pageSize);
        return Ok(trips);
    }

    /// <summary>
    /// Get active trip for current user
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveTrip()
    {
        var userId = GetCurrentUserId();
        var trip = await _tripService.GetActiveTripByUserAsync(userId);
        
        if (trip == null)
            return NotFound(new { message = "No active trip found" });

        return Ok(trip);
    }

    /// <summary>
    /// Get trip by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTripById(Guid id)
    {
        try
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            
            // Users can only see their own trips, admins can see all
            if (trip.UserId != GetCurrentUserId() && !User.IsInRole("admin"))
                return Forbid();

            return Ok(trip);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Trip with ID {id} not found" });
        }
    }

    /// <summary>
    /// Start a new trip
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StartTrip([FromBody] StartTripRequest request)
    {
        var userId = GetCurrentUserId();
        var httpClient = GetAuthenticatedHttpClient();

        var activeTrip = await _tripService.GetActiveTripByUserAsync(userId);
        if (activeTrip != null)
            return BadRequest(new { error = "User already has an active trip" });

        var vehicleId = request.VehicleId;

        var vehicleResponse = await httpClient.GetAsync(
            $"http://vehiclemanagement-api:5002/api/v1/vehicles/{vehicleId}");
        if (!vehicleResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get vehicle {VehicleId}. Status: {StatusCode}, Reason: {Reason}",
                vehicleId, vehicleResponse.StatusCode, vehicleResponse.ReasonPhrase);
            return StatusCode((int)vehicleResponse.StatusCode, 
                new { error = $"Failed to get vehicle {vehicleId}" });
        }

        var vehicleDto = await vehicleResponse.Content.ReadFromJsonAsync<VehicleDto>();

        var vehicleTypeResponse = await httpClient.GetAsync(
            $"http://vehiclemanagement-api:5002/api/v1/vehicletypes");
        if (!vehicleTypeResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get vehicle types. Status: {StatusCode}, Reason: {Reason}",
                vehicleTypeResponse.StatusCode, vehicleTypeResponse.ReasonPhrase);
            return StatusCode((int)vehicleTypeResponse.StatusCode, 
                new { error = "Failed to get vehicle types" });
        }

        var vehicleTypeDtos = await vehicleTypeResponse.Content.ReadFromJsonAsync<VehicleTypeDto[]>();
        var vehicleType = vehicleTypeDtos.FirstOrDefault(v => v.Id == vehicleDto.VehicleTypeId);

        if (vehicleType == null)
        {
            _logger.LogError("Vehicle type {VehicleTypeId} not found", vehicleDto.VehicleTypeId);
            return NotFound(new { error = "Vehicle type not found" });
        }

        var vehicleStatusUpdateResponse = await httpClient.PutAsJsonAsync(
            $"http://vehiclemanagement-api:5002/api/v1/vehicles/{vehicleId}/status",
            new UpdateVehicleStatusRequest { Status = "in_use" }
        );

        if (!vehicleStatusUpdateResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update vehicle status. Status: {StatusCode}, Reason: {Reason}",
                vehicleStatusUpdateResponse.StatusCode, vehicleStatusUpdateResponse.ReasonPhrase);
            return StatusCode((int)vehicleStatusUpdateResponse.StatusCode, 
                new { error = "Failed to update vehicle status" });
        }

        var commandRequest = new SendCommandRequest
        {
            CommandType = "unlock"
        };
        
        var commandResponse = await httpClient.PostAsJsonAsync(
            $"http://iotservice-api:5005/api/v1/commands/vehicles/{vehicleId}",
            commandRequest);
        
        if (!commandResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create iot command. Status: {StatusCode}, Reason: {Reason}",
                commandResponse.StatusCode, commandResponse.ReasonPhrase);
            return StatusCode((int)commandResponse.StatusCode, 
                new { error = "Failed to iot command" });
        }

        var startTripRequest = new StartTripWithPriceRequest
        {
            VehicleId = request.VehicleId,
            StartLongitude = request.StartLongitude,
            StartLatitude = request.StartLatitude,
            StartStationId = request.StartStationId,
            RatePerMinute = vehicleType.BasePricePerMinute,
            UnlockFee = vehicleType.UnlockFee,
        };
        
        var trip = await _tripService.StartTripAsync(userId, startTripRequest);

        return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, trip);
    }

    /// <summary>
    /// End a trip
    /// </summary>
    [HttpPost("{id}/end")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EndTrip(Guid id, [FromBody] EndTripRequest request)
    {
        var httpClient = GetAuthenticatedHttpClient();
        
        var trip = await _tripService.GetTripByIdAsync(id);
        if (trip == null)
            return NotFound(new { error = $"Trip with ID {id} not found" });

        if (trip.UserId != GetCurrentUserId())
            return Forbid();

        var updatedTrip = await _tripService.EndTripAsync(id, request);
        if (updatedTrip == null)
            return BadRequest(new { error = "Failed to end trip" });
        
        var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        _logger.LogInformation("Access token being sent: {Token}", accessToken);
        
        var paymentMethodResponse = await httpClient.GetAsync(
            "http://paymentservice-api:5004/api/v1/paymentmethods/my-methods/default");

        if (!paymentMethodResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get payment method. Status: {StatusCode}, Reason: {Reason}",
                paymentMethodResponse.StatusCode, paymentMethodResponse.ReasonPhrase);
            return StatusCode((int)paymentMethodResponse.StatusCode, 
                new { error = "Failed to get payment method" });
        }
        
        var paymentMethod = await paymentMethodResponse.Content.ReadFromJsonAsync<PaymentMethodDto>();

        var paymentRequest = new CreatePaymentRequest
        {
            TripId = id,
            Amount = updatedTrip.TotalFare ?? 0m,
            Currency = "EUR",
            PaymentMethodId = paymentMethod.Id
        };
        
        var paymentResponse = await httpClient.PostAsJsonAsync(
            "http://paymentservice-api:5004/api/v1/payments",
            paymentRequest);
        
        if (!paymentResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to make payment. Status: {StatusCode}, Reason: {Reason}",
                paymentResponse.StatusCode, paymentResponse.ReasonPhrase);
            return StatusCode((int)paymentResponse.StatusCode, 
                new { error = "Failed to make payment" });
        }
        
        var vehicleStatusUpdateResponse = await httpClient.PutAsJsonAsync(
            $"http://vehiclemanagement-api:5002/api/v1/vehicles/{updatedTrip.VehicleId}/status",
            new UpdateVehicleStatusRequest { Status = "available" }
        );

        if (!vehicleStatusUpdateResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update vehicle status. Status: {StatusCode}, Reason: {Reason}",
                vehicleStatusUpdateResponse.StatusCode, vehicleStatusUpdateResponse.ReasonPhrase);
            return StatusCode((int)vehicleStatusUpdateResponse.StatusCode, 
                new { error = "Failed to update vehicle status" });
        }
        
        var commandRequest = new SendCommandRequest
        {
            CommandType = "lock"
            
        };
        
        var commandResponse = await httpClient.PostAsJsonAsync(
            $"http://iotservice-api:5005/api/v1/commands/vehicles/{updatedTrip.VehicleId}",
            commandRequest);
        
        if (!commandResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create iot command. Status: {StatusCode}, Reason: {Reason}",
                commandResponse.StatusCode, commandResponse.ReasonPhrase);
            return StatusCode((int)commandResponse.StatusCode, 
                new { error = "Failed to iot command" });
        }

        return Ok(updatedTrip);
    }

    /// <summary>
    /// Cancel a trip
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelTrip(Guid id, [FromBody] CancelTripRequest request)
    {
        var trip = await _tripService.GetTripByIdAsync(id);
        if (trip == null)
            return NotFound(new { error = $"Trip with ID {id} not found" });

        if (trip.UserId != GetCurrentUserId())
            return Forbid();

        var success = await _tripService.CancelTripAsync(id, request.Reason);
        if (!success)
            return BadRequest(new { error = "Failed to cancel trip" });

        return Ok(new { message = "Trip cancelled successfully" });
    }

    /// <summary>
    /// Add location to trip
    /// </summary>
    [HttpPost("{id}/locations")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddTripLocation(Guid id, [FromBody] AddTripLocationRequest request)
    {
        var success = await _tripService.AddTripLocationAsync(id, request);
        if (!success)
            return BadRequest(new { error = "Failed to add location" });

        return Created($"/api/v1/trips/{id}/locations", new { message = "Location added successfully" });
    }

    /// <summary>
    /// Get trip locations
    /// </summary>
    [HttpGet("{id}/locations")]
    [ProducesResponseType(typeof(IEnumerable<TripLocationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTripLocations(Guid id)
    {
        var locations = await _tripService.GetTripLocationsAsync(id);
        return Ok(locations);
    }

    /// <summary>
    /// Rate a trip
    /// </summary>
    [HttpPost("{id}/rate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RateTrip(Guid id, [FromBody] RateTripRequest request)
    {
        var trip = await _tripService.GetTripByIdAsync(id);
        if (trip == null)
            return NotFound(new { error = $"Trip with ID {id} not found" });

        if (trip.UserId != GetCurrentUserId())
            return Forbid();

        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new { error = "Rating must be between 1 and 5" });

        var success = await _tripService.RateTripAsync(id, request.Rating, request.Review);
        if (!success)
            return BadRequest(new { error = "Failed to rate trip" });

        return Ok(new { message = "Trip rated successfully" });
    }

    /// <summary>
    /// Get current user's trip statistics
    /// </summary>
    [HttpGet("statistics/me")]
    [ProducesResponseType(typeof(TripStatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var userId = GetCurrentUserId();
        var stats = await _tripService.GetUserStatisticsAsync(userId, startDate, endDate);
        return Ok(stats);
    }

    /// <summary>
    /// Get system statistics (Admin only)
    /// </summary>
    [HttpGet("statistics/system")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(TripStatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var stats = await _tripService.GetSystemStatisticsAsync(startDate, endDate);
        return Ok(stats);
    }
}