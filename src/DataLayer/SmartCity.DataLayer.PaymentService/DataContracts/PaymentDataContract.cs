using System.Runtime.Serialization;

namespace SmartCity.DataLayer.PaymentService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class PaymentDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid UserId { get; set; }

    [DataMember(Order = 3)]
    public Guid? TripId { get; set; }

    [DataMember(Order = 4)]
    public decimal Amount { get; set; }

    [DataMember(Order = 5)]
    public string Currency { get; set; } = "USD";

    [DataMember(Order = 6)]
    public string PaymentMethod { get; set; } = string.Empty;

    [DataMember(Order = 7)]
    public string Status { get; set; } = "pending";

    [DataMember(Order = 8)]
    public string? TransactionId { get; set; }

    [DataMember(Order = 9)]
    public string? PaymentGateway { get; set; }

    [DataMember(Order = 10)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 11)]
    public DateTime? ProcessedAt { get; set; }

    [DataMember(Order = 12)]
    public string? FailureReason { get; set; }

    [DataMember(Order = 13)]
    public Guid? PaymentMethodId { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class PaymentMethodDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid UserId { get; set; }

    [DataMember(Order = 3)]
    public string Type { get; set; } = string.Empty; // credit_card, debit_card, paypal, apple_pay

    [DataMember(Order = 4)]
    public string? CardBrand { get; set; } // visa, mastercard, amex

    [DataMember(Order = 5)]
    public string? Last4Digits { get; set; }

    [DataMember(Order = 6)]
    public string? ExpiryMonth { get; set; }

    [DataMember(Order = 7)]
    public string? ExpiryYear { get; set; }

    [DataMember(Order = 8)]
    public string? CardholderName { get; set; }

    [DataMember(Order = 9)]
    public bool IsDefault { get; set; }

    [DataMember(Order = 10)]
    public string Status { get; set; } = "active";

    [DataMember(Order = 11)]
    public string? PaymentToken { get; set; }

    [DataMember(Order = 12)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 13)]
    public DateTime? VerifiedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class RefundDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid PaymentId { get; set; }

    [DataMember(Order = 3)]
    public decimal Amount { get; set; }

    [DataMember(Order = 4)]
    public string Reason { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string Status { get; set; } = "pending";

    [DataMember(Order = 6)]
    public string? RefundTransactionId { get; set; }

    [DataMember(Order = 7)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 8)]
    public DateTime? ProcessedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class PaymentAuthorizationDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public Guid UserId { get; set; }

    [DataMember(Order = 3)]
    public Guid PaymentMethodId { get; set; }

    [DataMember(Order = 4)]
    public decimal Amount { get; set; }

    [DataMember(Order = 5)]
    public string Currency { get; set; } = "USD";

    [DataMember(Order = 6)]
    public string Status { get; set; } = "authorized";

    [DataMember(Order = 7)]
    public string? AuthorizationCode { get; set; }

    [DataMember(Order = 8)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 9)]
    public DateTime ExpiresAt { get; set; }

    [DataMember(Order = 10)]
    public DateTime? CapturedAt { get; set; }

    [DataMember(Order = 11)]
    public DateTime? ReleasedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class CreatePaymentRequest
{
    [DataMember(Order = 1)]
    public Guid UserId { get; set; }

    [DataMember(Order = 2)]
    public Guid? TripId { get; set; }

    [DataMember(Order = 3)]
    public decimal Amount { get; set; }

    [DataMember(Order = 4)]
    public string Currency { get; set; } = "USD";

    [DataMember(Order = 5)]
    public Guid PaymentMethodId { get; set; }

    [DataMember(Order = 6)]
    public string? Description { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public class CreatePaymentMethodRequest
{
    [DataMember(Order = 1)]
    public Guid UserId { get; set; }

    [DataMember(Order = 2)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string? CardBrand { get; set; }

    [DataMember(Order = 4)]
    public string? Last4Digits { get; set; }

    [DataMember(Order = 5)]
    public string? ExpiryMonth { get; set; }

    [DataMember(Order = 6)]
    public string? ExpiryYear { get; set; }

    [DataMember(Order = 7)]
    public string? CardholderName { get; set; }

    [DataMember(Order = 8)]
    public string PaymentToken { get; set; } = string.Empty;

    [DataMember(Order = 9)]
    public bool IsDefault { get; set; }
}