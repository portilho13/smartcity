using SmartCity.PaymentService.Core.DTOs;
using SmartCity.DataLayer.PaymentService.Contracts;
using SmartCity.DataLayer.PaymentService.DataContracts;
using Microsoft.Extensions.Logging;
using CreatePaymentMethodRequest = SmartCity.PaymentService.Core.DTOs.CreatePaymentMethodRequest;
using CreatePaymentRequest = SmartCity.PaymentService.Core.DTOs.CreatePaymentRequest;

namespace SmartCity.PaymentService.Core.Services;

public interface IPaymentService
{
    Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByUserAsync(Guid userId, int page, int pageSize);
    Task<IEnumerable<PaymentDto>> GetPaymentsByTripAsync(Guid tripId);
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(int page, int pageSize);
    Task<PaymentDto> CreatePaymentAsync(Guid userId, CreatePaymentRequest request);
    Task<bool> ProcessPaymentAsync(Guid paymentId, string? transactionId);
    Task<bool> FailPaymentAsync(Guid paymentId, string reason);
    Task<bool> CancelPaymentAsync(Guid paymentId, string reason);
    
    Task<PaymentMethodDto?> GetPaymentMethodByIdAsync(Guid paymentMethodId);
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsByUserAsync(Guid userId);
    Task<PaymentMethodDto?> GetDefaultPaymentMethodAsync(Guid userId);
    Task<PaymentMethodDto> CreatePaymentMethodAsync(Guid userId, CreatePaymentMethodRequest request);
    Task<bool> DeletePaymentMethodAsync(Guid paymentMethodId);
    Task<bool> SetDefaultPaymentMethodAsync(Guid userId, Guid paymentMethodId);
    
    Task<PaymentAuthorizationDto> AuthorizePaymentAsync(Guid userId, AuthorizePaymentRequest request);
    Task<bool> CaptureAuthorizationAsync(Guid authorizationId);
    Task<bool> ReleaseAuthorizationAsync(Guid authorizationId);
    
    Task<RefundDto> CreateRefundAsync(Guid paymentId, CreateRefundRequest request);
    Task<RefundDto?> GetRefundByIdAsync(Guid refundId);
    Task<IEnumerable<RefundDto>> GetRefundsByPaymentAsync(Guid paymentId);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentDataService _soapClient;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentDataService soapClient, ILogger<PaymentService> logger)
    {
        _soapClient = soapClient;
        _logger = logger;
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId)
    {
        var paymentData = await _soapClient.GetPaymentByIdAsync(paymentId);
        if (paymentData == null)
            throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

        return MapToDto(paymentData);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByUserAsync(Guid userId, int page, int pageSize)
    {
        var paymentsData = await _soapClient.GetPaymentsByUserAsync(userId, page, pageSize);
        return paymentsData.Select(MapToDto);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByTripAsync(Guid tripId)
    {
        var paymentsData = await _soapClient.GetPaymentsByTripAsync(tripId);
        return paymentsData.Select(MapToDto);
    }

    public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(int page, int pageSize)
    {
        var paymentsData = await _soapClient.GetAllPaymentsAsync(page, pageSize);
        return paymentsData.Select(MapToDto);
    }

    public async Task<PaymentDto> CreatePaymentAsync(Guid userId, CreatePaymentRequest request)
    {
        var createRequest = new SmartCity.DataLayer.PaymentService.DataContracts.CreatePaymentRequest
        {
            UserId = userId,
            TripId = request.TripId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethodId = request.PaymentMethodId,
            Description = request.Description
        };

        var paymentData = await _soapClient.CreatePaymentAsync(createRequest);
        return MapToDto(paymentData);
    }

    public async Task<bool> ProcessPaymentAsync(Guid paymentId, string? transactionId)
    {
        return await _soapClient.UpdatePaymentStatusAsync(paymentId, "completed", transactionId, null);
    }

    public async Task<bool> FailPaymentAsync(Guid paymentId, string reason)
    {
        return await _soapClient.UpdatePaymentStatusAsync(paymentId, "failed", null, reason);
    }

    public async Task<bool> CancelPaymentAsync(Guid paymentId, string reason)
    {
        return await _soapClient.CancelPaymentAsync(paymentId, reason);
    }

    public async Task<PaymentMethodDto?> GetPaymentMethodByIdAsync(Guid paymentMethodId)
    {
        var methodData = await _soapClient.GetPaymentMethodByIdAsync(paymentMethodId);
        return methodData != null ? MapToDto(methodData) : null;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsByUserAsync(Guid userId)
    {
        var methodsData = await _soapClient.GetPaymentMethodsByUserAsync(userId);
        return methodsData.Select(MapToDto);
    }

    public async Task<PaymentMethodDto?> GetDefaultPaymentMethodAsync(Guid userId)
    {
        var methodData = await _soapClient.GetDefaultPaymentMethodAsync(userId);
        return methodData != null ? MapToDto(methodData) : null;
    }

    public async Task<PaymentMethodDto> CreatePaymentMethodAsync(Guid userId, CreatePaymentMethodRequest request)
    {
        var createRequest = new SmartCity.DataLayer.PaymentService.DataContracts.CreatePaymentMethodRequest
        {
            UserId = userId,
            Type = request.Type,
            CardBrand = request.CardBrand,
            Last4Digits = request.Last4Digits,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            CardholderName = request.CardholderName,
            PaymentToken = request.PaymentToken,
            IsDefault = request.IsDefault
        };

        var methodData = await _soapClient.CreatePaymentMethodAsync(createRequest);
        return MapToDto(methodData);
    }

    public async Task<bool> DeletePaymentMethodAsync(Guid paymentMethodId)
    {
        return await _soapClient.DeletePaymentMethodAsync(paymentMethodId);
    }

    public async Task<bool> SetDefaultPaymentMethodAsync(Guid userId, Guid paymentMethodId)
    {
        return await _soapClient.SetDefaultPaymentMethodAsync(userId, paymentMethodId);
    }

    public async Task<PaymentAuthorizationDto> AuthorizePaymentAsync(Guid userId, AuthorizePaymentRequest request)
    {
        var authData = await _soapClient.AuthorizePaymentAsync(
            userId, 
            request.PaymentMethodId, 
            request.Amount, 
            request.Currency
        );
        return MapToDto(authData);
    }

    public async Task<bool> CaptureAuthorizationAsync(Guid authorizationId)
    {
        return await _soapClient.CaptureAuthorizationAsync(authorizationId);
    }

    public async Task<bool> ReleaseAuthorizationAsync(Guid authorizationId)
    {
        return await _soapClient.ReleaseAuthorizationAsync(authorizationId);
    }

    public async Task<RefundDto> CreateRefundAsync(Guid paymentId, CreateRefundRequest request)
    {
        var refundData = await _soapClient.CreateRefundAsync(paymentId, request.Amount, request.Reason);
        return MapToDto(refundData);
    }

    public async Task<RefundDto?> GetRefundByIdAsync(Guid refundId)
    {
        var refundData = await _soapClient.GetRefundByIdAsync(refundId);
        return refundData != null ? MapToDto(refundData) : null;
    }

    public async Task<IEnumerable<RefundDto>> GetRefundsByPaymentAsync(Guid paymentId)
    {
        var refundsData = await _soapClient.GetRefundsByPaymentAsync(paymentId);
        return refundsData.Select(MapToDto);
    }

    // Mapping methods
    private static PaymentDto MapToDto(PaymentDataContract data)
    {
        return new PaymentDto
        {
            Id = data.Id,
            UserId = data.UserId,
            TripId = data.TripId,
            Amount = data.Amount,
            Currency = data.Currency,
            PaymentMethod = data.PaymentMethod,
            Status = data.Status,
            TransactionId = data.TransactionId,
            PaymentGateway = data.PaymentGateway,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
            FailureReason = data.FailureReason,
            PaymentMethodId = data.PaymentMethodId
        };
    }

    private static PaymentMethodDto MapToDto(PaymentMethodDataContract data)
    {
        return new PaymentMethodDto
        {
            Id = data.Id,
            UserId = data.UserId,
            Type = data.Type,
            CardBrand = data.CardBrand,
            Last4Digits = data.Last4Digits,
            ExpiryMonth = data.ExpiryMonth,
            ExpiryYear = data.ExpiryYear,
            CardholderName = data.CardholderName,
            IsDefault = data.IsDefault,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            VerifiedAt = data.VerifiedAt
        };
    }

    private static RefundDto MapToDto(RefundDataContract data)
    {
        return new RefundDto
        {
            Id = data.Id,
            PaymentId = data.PaymentId,
            Amount = data.Amount,
            Reason = data.Reason,
            Status = data.Status,
            RefundTransactionId = data.RefundTransactionId,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt
        };
    }

    private static PaymentAuthorizationDto MapToDto(PaymentAuthorizationDataContract data)
    {
        return new PaymentAuthorizationDto
        {
            Id = data.Id,
            UserId = data.UserId,
            PaymentMethodId = data.PaymentMethodId,
            Amount = data.Amount,
            Currency = data.Currency,
            Status = data.Status,
            AuthorizationCode = data.AuthorizationCode,
            CreatedAt = data.CreatedAt,
            ExpiresAt = data.ExpiresAt,
            CapturedAt = data.CapturedAt,
            ReleasedAt = data.ReleasedAt
        };
    }
}