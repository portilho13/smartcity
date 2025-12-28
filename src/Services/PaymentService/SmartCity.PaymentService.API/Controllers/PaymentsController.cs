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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.PaymentService.Core.DTOs;
using SmartCity.PaymentService.Core.Services;
using System.Security.Claims;

namespace SmartCity.PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
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
    /// Get all payments (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(page, pageSize);
        return Ok(payments);
    }

    /// <summary>
    /// Get current user's payments
    /// </summary>
    [HttpGet("my-payments")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        var payments = await _paymentService.GetPaymentsByUserAsync(userId, page, pageSize);
        return Ok(payments);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            // Users can only see their own payments, admins can see all
            if (payment.UserId != GetCurrentUserId() && !User.IsInRole("admin"))
                return Forbid();

            return Ok(payment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Payment with ID {id} not found" });
        }
    }

    /// <summary>
    /// Get payments for a trip
    /// </summary>
    [HttpGet("trips/{tripId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByTrip(Guid tripId)
    {
        var payments = await _paymentService.GetPaymentsByTripAsync(tripId);
        return Ok(payments);
    }

    /// <summary>
    /// Create a payment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        var userId = GetCurrentUserId();
        var payment = await _paymentService.CreatePaymentAsync(userId, request);

        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
    }

    /// <summary>
    /// Process a payment (mark as completed)
    /// </summary>
    [HttpPost("{id}/process")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessPayment(Guid id, [FromBody] ProcessPaymentRequest request)
    {
        var success = await _paymentService.ProcessPaymentAsync(id, request.TransactionId);
        if (!success)
            return NotFound(new { error = $"Payment with ID {id} not found" });

        return Ok(new { message = "Payment processed successfully" });
    }

    /// <summary>
    /// Cancel a payment
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelPayment(Guid id, [FromBody] CreateRefundRequest request)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment.UserId != GetCurrentUserId() && !User.IsInRole("admin"))
            return Forbid();

        var success = await _paymentService.CancelPaymentAsync(id, request.Reason);
        if (!success)
            return NotFound(new { error = $"Payment with ID {id} not found or cannot be cancelled" });

        return Ok(new { message = "Payment cancelled successfully" });
    }
}