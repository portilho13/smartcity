using Dapper;
using SmartCity.DataLayer.UserService.DataContracts;
using SmartCity.DataLayer.UserService.Infrastructure;

namespace SmartCity.DataLayer.UserService.Repositories;

public class UserRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DatabaseConnectionFactory connectionFactory, ILogger<UserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<UserDataContract?> GetByIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT id, email, first_name as FirstName, last_name as LastName,
                   phone_number as PhoneNumber, date_of_birth as DateOfBirth,
                   profile_picture_url as ProfilePictureUrl, email_verified as EmailVerified,
                   account_status as AccountStatus, user_type as UserType,
                   created_at as CreatedAt, updated_at as UpdatedAt
            FROM users WHERE id = @UserId AND deleted_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserDataContract>(sql, new { UserId = userId });
    }

    public async Task<UserDataContract?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT id, email, first_name as FirstName, last_name as LastName,
                   phone_number as PhoneNumber, date_of_birth as DateOfBirth,
                   profile_picture_url as ProfilePictureUrl, email_verified as EmailVerified,
                   account_status as AccountStatus, user_type as UserType,
                   created_at as CreatedAt, updated_at as UpdatedAt
            FROM users WHERE LOWER(email) = LOWER(@Email) AND deleted_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserDataContract>(sql, new { Email = email });
    }

    public async Task<Guid> CreateAsync(CreateUserDataContract userData)
    {
        const string sql = @"
            INSERT INTO users (email, password_hash, first_name, last_name, phone_number, date_of_birth)
            VALUES (@Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @DateOfBirth)
            RETURNING id";

        using var connection = _connectionFactory.CreateConnection();
        var userId = await connection.QuerySingleAsync<Guid>(sql, userData);
        _logger.LogInformation("User created: {UserId}", userId);
        return userId;
    }

    public async Task<bool> UpdateAsync(UserDataContract userData)
    {
        const string sql = @"
            UPDATE users SET 
                first_name = @FirstName, last_name = @LastName,
                phone_number = @PhoneNumber, date_of_birth = @DateOfBirth,
                profile_picture_url = @ProfilePictureUrl, email_verified = @EmailVerified,
                account_status = @AccountStatus, user_type = @UserType,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id AND deleted_at IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, userData);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        const string sql = "UPDATE users SET deleted_at = CURRENT_TIMESTAMP WHERE id = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { UserId = userId });
        return rows > 0;
    }

    public async Task<UserDataContract[]> GetAllAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        const string sql = @"
            SELECT id, email, first_name as FirstName, last_name as LastName,
                   phone_number as PhoneNumber, date_of_birth as DateOfBirth,
                   profile_picture_url as ProfilePictureUrl, email_verified as EmailVerified,
                   account_status as AccountStatus, user_type as UserType,
                   created_at as CreatedAt, updated_at as UpdatedAt
            FROM users WHERE deleted_at IS NULL
            ORDER BY created_at DESC LIMIT @PageSize OFFSET @Offset";

        using var connection = _connectionFactory.CreateConnection();
        var users = await connection.QueryAsync<UserDataContract>(sql, new { PageSize = pageSize, Offset = offset });
        return users.ToArray();
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string passwordHash)
    {
        const string sql = @"
            SELECT COUNT(*) FROM users 
            WHERE LOWER(email) = LOWER(@Email) AND password_hash = @PasswordHash 
              AND deleted_at IS NULL AND account_status = 'active'";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email, PasswordHash = passwordHash });
        return count > 0;
    }
}