using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.PaymentService.Core.DTOs;
using SmartCity.PaymentService.Core.Services;

namespace SmartCity.PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RefundsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<RefundsController> _logger;

    public RefundsController(IPaymentService paymentService, ILogger<RefundsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Get refund by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RefundDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRefundById(Guid id)
    {
        var refund = await _paymentService.GetRefundByIdAsync(id);
        
        if (refund == null)
            return NotFound(new { error = $"Refund with ID {id} not found" });

        return Ok(refund);
    }

    /// <summary>
    /// Get refunds for a payment
    /// </summary>
    [HttpGet("payments/{paymentId}")]
    [ProducesResponseType(typeof(IEnumerable<RefundDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRefundsByPayment(Guid paymentId)
    {
        var refunds = await _paymentService.GetRefundsByPaymentAsync(paymentId);
        return Ok(refunds);
    }

    /// <summary>
    /// Create a refund
    /// </summary>
    [HttpPost("payments/{paymentId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(RefundDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRefund(Guid paymentId, [FromBody] CreateRefundRequest request)
    {
        var refund = await _paymentService.CreateRefundAsync(paymentId, request);
        
        return CreatedAtAction(nameof(GetRefundById), new { id = refund.Id }, refund);
    }
}