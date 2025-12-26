using SmartCity.UserManagement.Core.DTOs;
using SmartCity.DataLayer.UserService.Contracts;
using SmartCity.DataLayer.UserService.DataContracts;
using SmartCity.Shared.Common.Utils;
using SmartCity.Shared.Security;
using Microsoft.Extensions.Logging;

namespace SmartCity.UserManagement.Core.Services;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize);
    Task<Guid> CreateUserAsync(RegisterRequest request);
    Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<LoginResponse> AuthenticateAsync(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly IUserDataService _soapClient;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserDataService soapClient, IJwtTokenService jwtTokenService, ILogger<UserService> logger)
    {
        _soapClient = soapClient;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var userData = await _soapClient.GetUserByIdAsync(userId);
        if (userData == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        return MapToDto(userData);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var userData = await _soapClient.GetUserByEmailAsync(email);
        return userData != null ? MapToDto(userData) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize)
    {
        var usersData = await _soapClient.GetAllUsersAsync(page, pageSize);
        return usersData.Select(MapToDto);
    }

    public async Task<Guid> CreateUserAsync(RegisterRequest request)
    {
        var existingUser = await GetUserByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        var passwordHash = PasswordHasher.HashPassword(request.Password);
        var createData = new CreateUserDataContract
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth
        };

        return await _soapClient.CreateUserAsync(createData);
    }

    public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var userData = await _soapClient.GetUserByIdAsync(userId);
        if (userData == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        userData.FirstName = request.FirstName ?? userData.FirstName;
        userData.LastName = request.LastName ?? userData.LastName;
        userData.PhoneNumber = request.PhoneNumber ?? userData.PhoneNumber;
        userData.DateOfBirth = request.DateOfBirth ?? userData.DateOfBirth;

        return await _soapClient.UpdateUserAsync(userData);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        return await _soapClient.DeleteUserAsync(userId);
    }

    public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
    {
        var passwordHash = PasswordHasher.HashPassword(request.Password);
        var isValid = await _soapClient.ValidateUserCredentialsAsync(request.Email, passwordHash);

        if (!isValid)
            throw new UnauthorizedAccessException("Invalid email or password");

        var user = await GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.UserType);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = user
        };
    }

    private static UserDto MapToDto(UserDataContract data)
    {
        return new UserDto
        {
            Id = data.Id,
            Email = data.Email,
            FirstName = data.FirstName,
            LastName = data.LastName,
            PhoneNumber = data.PhoneNumber,
            DateOfBirth = data.DateOfBirth,
            AccountStatus = data.AccountStatus,
            UserType = data.UserType,
            EmailVerified = data.EmailVerified,
            CreatedAt = data.CreatedAt
        };
    }
}