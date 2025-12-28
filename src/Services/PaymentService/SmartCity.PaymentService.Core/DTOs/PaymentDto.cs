namespace SmartCity.PaymentService.Core.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TripId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public Guid? PaymentMethodId { get; set; }
}

public class PaymentMethodDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? CardBrand { get; set; }
    public string? Last4Digits { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public string? CardholderName { get; set; }
    public bool IsDefault { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? RefundTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class PaymentAuthorizationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "authorized";
    public string? AuthorizationCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CapturedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

public class CreatePaymentRequest
{
    public Guid? TripId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public Guid PaymentMethodId { get; set; }
    public string? Description { get; set; }
}

public class CreatePaymentMethodRequest
{
    public string Type { get; set; } = string.Empty;
    public string? CardBrand { get; set; }
    public string? Last4Digits { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public string? CardholderName { get; set; }
    public string PaymentToken { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class AuthorizePaymentRequest
{
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
}

public class CreateRefundRequest
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ProcessPaymentRequest
{
    public string? TransactionId { get; set; }
}