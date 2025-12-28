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

using System.ServiceModel;
using SmartCity.DataLayer.PaymentService.DataContracts;

namespace SmartCity.DataLayer.PaymentService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/paymentdata/v1")]
public interface IPaymentDataService
{
    // Payment operations
    [OperationContract]
    Task<PaymentDataContract?> GetPaymentByIdAsync(Guid paymentId);

    [OperationContract]
    Task<PaymentDataContract[]> GetPaymentsByUserAsync(Guid userId, int pageNumber, int pageSize);

    [OperationContract]
    Task<PaymentDataContract[]> GetPaymentsByTripAsync(Guid tripId);

    [OperationContract]
    Task<PaymentDataContract[]> GetAllPaymentsAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<PaymentDataContract> CreatePaymentAsync(CreatePaymentRequest request);

    [OperationContract]
    Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status, string? transactionId, string? failureReason);

    [OperationContract]
    Task<bool> CancelPaymentAsync(Guid paymentId, string reason);

    // Payment Method operations
    [OperationContract]
    Task<PaymentMethodDataContract?> GetPaymentMethodByIdAsync(Guid paymentMethodId);

    [OperationContract]
    Task<PaymentMethodDataContract[]> GetPaymentMethodsByUserAsync(Guid userId);

    [OperationContract]
    Task<PaymentMethodDataContract?> GetDefaultPaymentMethodAsync(Guid userId);

    [OperationContract]
    Task<PaymentMethodDataContract> CreatePaymentMethodAsync(CreatePaymentMethodRequest request);

    [OperationContract]
    Task<bool> DeletePaymentMethodAsync(Guid paymentMethodId);

    [OperationContract]
    Task<bool> SetDefaultPaymentMethodAsync(Guid userId, Guid paymentMethodId);

    // Authorization operations
    [OperationContract]
    Task<PaymentAuthorizationDataContract> AuthorizePaymentAsync(Guid userId, Guid paymentMethodId, decimal amount, string currency);

    [OperationContract]
    Task<bool> CaptureAuthorizationAsync(Guid authorizationId);

    [OperationContract]
    Task<bool> ReleaseAuthorizationAsync(Guid authorizationId);

    // Refund operations
    [OperationContract]
    Task<RefundDataContract> CreateRefundAsync(Guid paymentId, decimal amount, string reason);

    [OperationContract]
    Task<RefundDataContract?> GetRefundByIdAsync(Guid refundId);

    [OperationContract]
    Task<RefundDataContract[]> GetRefundsByPaymentAsync(Guid paymentId);
}