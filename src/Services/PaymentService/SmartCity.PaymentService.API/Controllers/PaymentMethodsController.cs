using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.PaymentService.Core.DTOs;
using SmartCity.PaymentService.Core.Services;
using System.Security.Claims;

namespace SmartCity.PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentMethodsController> _logger;

    public PaymentMethodsController(IPaymentService paymentService, ILogger<PaymentMethodsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");
        
        return userId;
    }

    /// <summary>
    /// Get current user's payment methods
    /// </summary>
    [HttpGet("my-methods")]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPaymentMethods()
    {
        _logger.LogInformation("=== GetMyDefaultPaymentMethod endpoint hit ===");
        _logger.LogInformation("IsAuthenticated: {IsAuth}", User.Identity?.IsAuthenticated);
        _logger.LogInformation("Auth Type: {AuthType}", User.Identity?.AuthenticationType);
        _logger.LogInformation("Claims count: {Count}", User.Claims.Count());
    
        foreach (var claim in User.Claims)
        {
            _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }

        var userId = GetCurrentUserId();
        var methods = await _paymentService.GetPaymentMethodsByUserAsync(userId);
        return Ok(methods);
    }

    /// <summary>
    /// Get current user's default payment method
    /// </summary>
    [HttpGet("my-methods/default")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyDefaultPaymentMethod()
    {
        var userId = GetCurrentUserId();
        var method = await _paymentService.GetDefaultPaymentMethodAsync(userId);
        
        if (method == null)
            return NotFound(new { message = "No default payment method found" });

        return Ok(method);
    }

    /// <summary>
    /// Get payment method by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentMethodById(Guid id)
    {
        var method = await _paymentService.GetPaymentMethodByIdAsync(id);
        
        if (method == null)
            return NotFound(new { error = $"Payment method with ID {id} not found" });

        // Users can only see their own payment methods
        if (method.UserId != GetCurrentUserId() && !User.IsInRole("admin"))
            return Forbid();

        return Ok(method);
    }

    /// <summary>
    /// Add a new payment method
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodRequest request)
    {
        var userId = GetCurrentUserId();
        var method = await _paymentService.CreatePaymentMethodAsync(userId, request);
        
        return CreatedAtAction(nameof(GetPaymentMethodById), new { id = method.Id }, method);
    }

    /// <summary>
    /// Delete a payment method
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePaymentMethod(Guid id)
    {
        var method = await _paymentService.GetPaymentMethodByIdAsync(id);
        if (method == null)
            return NotFound(new { error = $"Payment method with ID {id} not found" });

        if (method.UserId != GetCurrentUserId())
            return Forbid();

        var success = await _paymentService.DeletePaymentMethodAsync(id);
        if (!success)
            return NotFound(new { error = $"Payment method with ID {id} not found" });

        return Ok(new { message = "Payment method deleted successfully" });
    }

    /// <summary>
    /// Set payment method as default
    /// </summary>
    [HttpPut("{id}/set-default")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultPaymentMethod(Guid id)
    {
        var userId = GetCurrentUserId();
        var method = await _paymentService.GetPaymentMethodByIdAsync(id);
        
        if (method == null)
            return NotFound(new { error = $"Payment method with ID {id} not found" });

        if (method.UserId != userId)
            return Forbid();

        var success = await _paymentService.SetDefaultPaymentMethodAsync(userId, id);
        if (!success)
            return NotFound(new { error = $"Payment method with ID {id} not found" });

        return Ok(new { message = "Default payment method updated successfully" });
    }
}