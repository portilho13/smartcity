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

using Dapper;
using SmartCity.DataLayer.PaymentService.DataContracts;
using SmartCity.DataLayer.PaymentService.Infrastructure;

namespace SmartCity.DataLayer.PaymentService.Repositories;

public class PaymentRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(DatabaseConnectionFactory connectionFactory, ILogger<PaymentRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // ==================== Payment Operations ====================

    public async Task<PaymentDataContract?> GetByIdAsync(Guid paymentId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, trip_id as TripId, amount, currency,
                payment_method as PaymentMethod, status, transaction_id as TransactionId,
                payment_gateway as PaymentGateway, created_at as CreatedAt,
                processed_at as ProcessedAt, failure_reason as FailureReason,
                payment_method_id as PaymentMethodId
            FROM payments
            WHERE id = @PaymentId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PaymentDataContract>(sql, new { PaymentId = paymentId });
    }

    public async Task<PaymentDataContract[]> GetByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, user_id as UserId, trip_id as TripId, amount, currency,
                payment_method as PaymentMethod, status, transaction_id as TransactionId,
                payment_gateway as PaymentGateway, created_at as CreatedAt,
                processed_at as ProcessedAt, failure_reason as FailureReason,
                payment_method_id as PaymentMethodId
            FROM payments
            WHERE user_id = @UserId
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var payments = await connection.QueryAsync<PaymentDataContract>(sql, new { UserId = userId, PageSize = pageSize, Offset = offset });
        return payments.ToArray();
    }

    public async Task<PaymentDataContract[]> GetByTripAsync(Guid tripId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, trip_id as TripId, amount, currency,
                payment_method as PaymentMethod, status, transaction_id as TransactionId,
                payment_gateway as PaymentGateway, created_at as CreatedAt,
                processed_at as ProcessedAt, failure_reason as FailureReason,
                payment_method_id as PaymentMethodId
            FROM payments
            WHERE trip_id = @TripId
            ORDER BY created_at DESC";

        using var connection = _connectionFactory.CreateConnection();
        var payments = await connection.QueryAsync<PaymentDataContract>(sql, new { TripId = tripId });
        return payments.ToArray();
    }

    public async Task<PaymentDataContract[]> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT 
                id, user_id as UserId, trip_id as TripId, amount, currency,
                payment_method as PaymentMethod, status, transaction_id as TransactionId,
                payment_gateway as PaymentGateway, created_at as CreatedAt,
                processed_at as ProcessedAt, failure_reason as FailureReason,
                payment_method_id as PaymentMethodId
            FROM payments
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var payments = await connection.QueryAsync<PaymentDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return payments.ToArray();
    }

    public async Task<PaymentDataContract> CreateAsync(CreatePaymentRequest request)
    {
        const string sql = @"
            INSERT INTO payments (
                user_id, trip_id, amount, currency, payment_method_id,
                status, created_at
            )
            VALUES (
                @UserId, @TripId, @Amount, @Currency, @PaymentMethodId,
                'pending', CURRENT_TIMESTAMP
            )
            RETURNING 
                id, user_id as UserId, trip_id as TripId, amount, currency,
                payment_method as PaymentMethod, status, transaction_id as TransactionId,
                payment_gateway as PaymentGateway, created_at as CreatedAt,
                processed_at as ProcessedAt, failure_reason as FailureReason,
                payment_method_id as PaymentMethodId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<PaymentDataContract>(sql, request);
    }

    public async Task<bool> UpdateStatusAsync(Guid paymentId, string status, string? transactionId, string? failureReason)
    {
        const string sql = @"
            UPDATE payments
            SET 
                status = @Status,
                transaction_id = COALESCE(@TransactionId, transaction_id),
                failure_reason = @FailureReason,
                processed_at = CASE WHEN @Status IN ('completed', 'failed') THEN CURRENT_TIMESTAMP ELSE processed_at END
            WHERE id = @PaymentId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { PaymentId = paymentId, Status = status, TransactionId = transactionId, FailureReason = failureReason });
        return rows > 0;
    }

    public async Task<bool> CancelAsync(Guid paymentId, string reason)
    {
        const string sql = @"
            UPDATE payments
            SET 
                status = 'cancelled',
                failure_reason = @Reason,
                processed_at = CURRENT_TIMESTAMP
            WHERE id = @PaymentId AND status = 'pending'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { PaymentId = paymentId, Reason = reason });
        return rows > 0;
    }

    // ==================== Payment Method Operations ====================

    public async Task<PaymentMethodDataContract?> GetPaymentMethodByIdAsync(Guid paymentMethodId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, type, card_brand as CardBrand,
                last_4_digits as Last4Digits, expiry_month as ExpiryMonth,
                expiry_year as ExpiryYear, cardholder_name as CardholderName,
                is_default as IsDefault, status, payment_token as PaymentToken,
                created_at as CreatedAt, verified_at as VerifiedAt
            FROM payment_methods
            WHERE id = @PaymentMethodId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PaymentMethodDataContract>(sql, new { PaymentMethodId = paymentMethodId });
    }

    public async Task<PaymentMethodDataContract[]> GetPaymentMethodsByUserAsync(Guid userId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, type, card_brand as CardBrand,
                last_4_digits as Last4Digits, expiry_month as ExpiryMonth,
                expiry_year as ExpiryYear, cardholder_name as CardholderName,
                is_default as IsDefault, status, payment_token as PaymentToken,
                created_at as CreatedAt, verified_at as VerifiedAt
            FROM payment_methods
            WHERE user_id = @UserId AND status = 'active'
            ORDER BY is_default DESC, created_at DESC";

        using var connection = _connectionFactory.CreateConnection();
        var methods = await connection.QueryAsync<PaymentMethodDataContract>(sql, new { UserId = userId });
        return methods.ToArray();
    }

    public async Task<PaymentMethodDataContract?> GetDefaultPaymentMethodAsync(Guid userId)
    {
        const string sql = @"
            SELECT 
                id, user_id as UserId, type, card_brand as CardBrand,
                last_4_digits as Last4Digits, expiry_month as ExpiryMonth,
                expiry_year as ExpiryYear, cardholder_name as CardholderName,
                is_default as IsDefault, status, payment_token as PaymentToken,
                created_at as CreatedAt, verified_at as VerifiedAt
            FROM payment_methods
            WHERE user_id = @UserId AND is_default = true AND status = 'active'
            LIMIT 1";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PaymentMethodDataContract>(sql, new { UserId = userId });
    }

    public async Task<PaymentMethodDataContract> CreatePaymentMethodAsync(CreatePaymentMethodRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // If this is set as default, unset other defaults
            if (request.IsDefault)
            {
                const string unsetDefaultSql = @"
                    UPDATE payment_methods
                    SET is_default = false
                    WHERE user_id = @UserId AND is_default = true";

                await connection.ExecuteAsync(unsetDefaultSql, new { request.UserId }, transaction);
            }

            const string insertSql = @"
                INSERT INTO payment_methods (
                    user_id, type, card_brand, last_4_digits, expiry_month, expiry_year,
                    cardholder_name, is_default, status, payment_token, created_at, verified_at
                )
                VALUES (
                    @UserId, @Type, @CardBrand, @Last4Digits, @ExpiryMonth, @ExpiryYear,
                    @CardholderName, @IsDefault, 'active', @PaymentToken, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
                )
                RETURNING 
                    id, user_id as UserId, type, card_brand as CardBrand,
                    last_4_digits as Last4Digits, expiry_month as ExpiryMonth,
                    expiry_year as ExpiryYear, cardholder_name as CardholderName,
                    is_default as IsDefault, status, payment_token as PaymentToken,
                    created_at as CreatedAt, verified_at as VerifiedAt";

            var paymentMethod = await connection.QuerySingleAsync<PaymentMethodDataContract>(insertSql, request, transaction);

            transaction.Commit();
            return paymentMethod;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeletePaymentMethodAsync(Guid paymentMethodId)
    {
        const string sql = @"
            UPDATE payment_methods
            SET status = 'deleted'
            WHERE id = @PaymentMethodId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { PaymentMethodId = paymentMethodId });
        return rows > 0;
    }

    public async Task<bool> SetDefaultPaymentMethodAsync(Guid userId, Guid paymentMethodId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Unset all defaults for user
            const string unsetSql = @"
                UPDATE payment_methods
                SET is_default = false
                WHERE user_id = @UserId";

            await connection.ExecuteAsync(unsetSql, new { UserId = userId }, transaction);

            // Set new default
            const string setSql = @"
                UPDATE payment_methods
                SET is_default = true
                WHERE id = @PaymentMethodId AND user_id = @UserId";

            var rows = await connection.ExecuteAsync(setSql, new { PaymentMethodId = paymentMethodId, UserId = userId }, transaction);

            transaction.Commit();
            return rows > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // ==================== Authorization Operations ====================

    public async Task<PaymentAuthorizationDataContract> AuthorizeAsync(Guid userId, Guid paymentMethodId, decimal amount, string currency)
    {
        const string sql = @"
            INSERT INTO payment_authorizations (
                user_id, payment_method_id, amount, currency, status,
                authorization_code, created_at, expires_at
            )
            VALUES (
                @UserId, @PaymentMethodId, @Amount, @Currency, 'authorized',
                @AuthorizationCode, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '7 days'
            )
            RETURNING 
                id, user_id as UserId, payment_method_id as PaymentMethodId,
                amount, currency, status, authorization_code as AuthorizationCode,
                created_at as CreatedAt, expires_at as ExpiresAt,
                captured_at as CapturedAt, released_at as ReleasedAt";

        var authCode = $"AUTH-{Guid.NewGuid():N}";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<PaymentAuthorizationDataContract>(
            sql,
            new { UserId = userId, PaymentMethodId = paymentMethodId, Amount = amount, Currency = currency, AuthorizationCode = authCode }
        );
    }

    public async Task<bool> CaptureAuthorizationAsync(Guid authorizationId)
    {
        const string sql = @"
            UPDATE payment_authorizations
            SET 
                status = 'captured',
                captured_at = CURRENT_TIMESTAMP
            WHERE id = @AuthorizationId AND status = 'authorized'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { AuthorizationId = authorizationId });
        return rows > 0;
    }

    public async Task<bool> ReleaseAuthorizationAsync(Guid authorizationId)
    {
        const string sql = @"
            UPDATE payment_authorizations
            SET 
                status = 'released',
                released_at = CURRENT_TIMESTAMP
            WHERE id = @AuthorizationId AND status = 'authorized'";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { AuthorizationId = authorizationId });
        return rows > 0;
    }

    // ==================== Refund Operations ====================

    public async Task<RefundDataContract> CreateRefundAsync(Guid paymentId, decimal amount, string reason)
    {
        const string sql = @"
            INSERT INTO refunds (
                payment_id, amount, reason, status, created_at
            )
            VALUES (
                @PaymentId, @Amount, @Reason, 'pending', CURRENT_TIMESTAMP
            )
            RETURNING 
                id, payment_id as PaymentId, amount, reason,
                status, refund_transaction_id as RefundTransactionId,
                created_at as CreatedAt, processed_at as ProcessedAt";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<RefundDataContract>(
            sql,
            new { PaymentId = paymentId, Amount = amount, Reason = reason }
        );
    }

    public async Task<RefundDataContract?> GetRefundByIdAsync(Guid refundId)
    {
        const string sql = @"
            SELECT 
                id, payment_id as PaymentId, amount, reason,
                status, refund_transaction_id as RefundTransactionId,
                created_at as CreatedAt, processed_at as ProcessedAt
            FROM refunds
            WHERE id = @RefundId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RefundDataContract>(sql, new { RefundId = refundId });
    }

    public async Task<RefundDataContract[]> GetRefundsByPaymentAsync(Guid paymentId)
    {
        const string sql = @"
            SELECT 
                id, payment_id as PaymentId, amount, reason,
                status, refund_transaction_id as RefundTransactionId,
                created_at as CreatedAt, processed_at as ProcessedAt
            FROM refunds
            WHERE payment_id = @PaymentId
            ORDER BY created_at DESC";

        using var connection = _connectionFactory.CreateConnection();
        var refunds = await connection.QueryAsync<RefundDataContract>(sql, new { PaymentId = paymentId });
        return refunds.ToArray();
    }
}