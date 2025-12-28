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

using CoreWCF;
using SmartCity.DataLayer.PaymentService.Contracts;
using SmartCity.DataLayer.PaymentService.DataContracts;
using SmartCity.DataLayer.PaymentService.Repositories;

namespace SmartCity.DataLayer.PaymentService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class PaymentDataService : IPaymentDataService
{
    private readonly PaymentRepository _repository;
    private readonly ILogger<PaymentDataService> _logger;

    public PaymentDataService(PaymentRepository repository, ILogger<PaymentDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PaymentDataContract?> GetPaymentByIdAsync(Guid paymentId)
    {
        _logger.LogInformation("SOAP: GetPaymentById {PaymentId}", paymentId);
        return await _repository.GetByIdAsync(paymentId);
    }

    public async Task<PaymentDataContract[]> GetPaymentsByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetPaymentsByUser {UserId} Page={Page} Size={Size}", userId, pageNumber, pageSize);
        return await _repository.GetByUserAsync(userId, pageNumber, pageSize);
    }

    public async Task<PaymentDataContract[]> GetPaymentsByTripAsync(Guid tripId)
    {
        _logger.LogInformation("SOAP: GetPaymentsByTrip {TripId}", tripId);
        return await _repository.GetByTripAsync(tripId);
    }

    public async Task<PaymentDataContract[]> GetAllPaymentsAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAllPayments Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<PaymentDataContract> CreatePaymentAsync(CreatePaymentRequest request)
    {
        _logger.LogInformation("SOAP: CreatePayment User={UserId} Amount={Amount}", request.UserId, request.Amount);
        return await _repository.CreateAsync(request);
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status, string? transactionId, string? failureReason)
    {
        _logger.LogInformation("SOAP: UpdatePaymentStatus {PaymentId} -> {Status}", paymentId, status);
        return await _repository.UpdateStatusAsync(paymentId, status, transactionId, failureReason);
    }

    public async Task<bool> CancelPaymentAsync(Guid paymentId, string reason)
    {
        _logger.LogInformation("SOAP: CancelPayment {PaymentId}", paymentId);
        return await _repository.CancelAsync(paymentId, reason);
    }

    public async Task<PaymentMethodDataContract?> GetPaymentMethodByIdAsync(Guid paymentMethodId)
    {
        _logger.LogInformation("SOAP: GetPaymentMethodById {PaymentMethodId}", paymentMethodId);
        return await _repository.GetPaymentMethodByIdAsync(paymentMethodId);
    }

    public async Task<PaymentMethodDataContract[]> GetPaymentMethodsByUserAsync(Guid userId)
    {
        _logger.LogInformation("SOAP: GetPaymentMethodsByUser {UserId}", userId);
        return await _repository.GetPaymentMethodsByUserAsync(userId);
    }

    public async Task<PaymentMethodDataContract?> GetDefaultPaymentMethodAsync(Guid userId)
    {
        _logger.LogInformation("SOAP: GetDefaultPaymentMethod {UserId}", userId);
        return await _repository.GetDefaultPaymentMethodAsync(userId);
    }

    public async Task<PaymentMethodDataContract> CreatePaymentMethodAsync(CreatePaymentMethodRequest request)
    {
        _logger.LogInformation("SOAP: CreatePaymentMethod User={UserId} Type={Type}", request.UserId, request.Type);
        return await _repository.CreatePaymentMethodAsync(request);
    }

    public async Task<bool> DeletePaymentMethodAsync(Guid paymentMethodId)
    {
        _logger.LogInformation("SOAP: DeletePaymentMethod {PaymentMethodId}", paymentMethodId);
        return await _repository.DeletePaymentMethodAsync(paymentMethodId);
    }

    public async Task<bool> SetDefaultPaymentMethodAsync(Guid userId, Guid paymentMethodId)
    {
        _logger.LogInformation("SOAP: SetDefaultPaymentMethod User={UserId} Method={PaymentMethodId}", userId, paymentMethodId);
        return await _repository.SetDefaultPaymentMethodAsync(userId, paymentMethodId);
    }

    public async Task<PaymentAuthorizationDataContract> AuthorizePaymentAsync(Guid userId, Guid paymentMethodId, decimal amount, string currency)
    {
        _logger.LogInformation("SOAP: AuthorizePayment User={UserId} Amount={Amount}", userId, amount);
        return await _repository.AuthorizeAsync(userId, paymentMethodId, amount, currency);
    }

    public async Task<bool> CaptureAuthorizationAsync(Guid authorizationId)
    {
        _logger.LogInformation("SOAP: CaptureAuthorization {AuthorizationId}", authorizationId);
        return await _repository.CaptureAuthorizationAsync(authorizationId);
    }

    public async Task<bool> ReleaseAuthorizationAsync(Guid authorizationId)
    {
        _logger.LogInformation("SOAP: ReleaseAuthorization {AuthorizationId}", authorizationId);
        return await _repository.ReleaseAuthorizationAsync(authorizationId);
    }

    public async Task<RefundDataContract> CreateRefundAsync(Guid paymentId, decimal amount, string reason)
    {
        _logger.LogInformation("SOAP: CreateRefund Payment={PaymentId} Amount={Amount}", paymentId, amount);
        return await _repository.CreateRefundAsync(paymentId, amount, reason);
    }

    public async Task<RefundDataContract?> GetRefundByIdAsync(Guid refundId)
    {
        _logger.LogInformation("SOAP: GetRefundById {RefundId}", refundId);
        return await _repository.GetRefundByIdAsync(refundId);
    }

    public async Task<RefundDataContract[]> GetRefundsByPaymentAsync(Guid paymentId)
    {
        _logger.LogInformation("SOAP: GetRefundsByPayment {PaymentId}", paymentId);
        return await _repository.GetRefundsByPaymentAsync(paymentId);
    }
}