using CoreWCF;
using SmartCity.DataLayer.UserService.Contracts;
using SmartCity.DataLayer.UserService.DataContracts;
using SmartCity.DataLayer.UserService.Repositories;

namespace SmartCity.DataLayer.UserService.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
public class UserDataService : IUserDataService
{
    private readonly UserRepository _repository;
    private readonly ILogger<UserDataService> _logger;

    public UserDataService(UserRepository repository, ILogger<UserDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UserDataContract?> GetUserByIdAsync(Guid userId)
    {
        _logger.LogInformation("SOAP: GetUserById {UserId}", userId);
        return await _repository.GetByIdAsync(userId);
    }

    public async Task<UserDataContract?> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("SOAP: GetUserByEmail {Email}", email);
        return await _repository.GetByEmailAsync(email);
    }

    public async Task<Guid> CreateUserAsync(CreateUserDataContract userData)
    {
        _logger.LogInformation("SOAP: CreateUser {Email}", userData.Email);
        return await _repository.CreateAsync(userData);
    }

    public async Task<bool> UpdateUserAsync(UserDataContract userData)
    {
        _logger.LogInformation("SOAP: UpdateUser {UserId}", userData.Id);
        return await _repository.UpdateAsync(userData);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        _logger.LogInformation("SOAP: DeleteUser {UserId}", userId);
        return await _repository.DeleteAsync(userId);
    }

    public async Task<UserDataContract[]> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("SOAP: GetAllUsers Page={Page} Size={Size}", pageNumber, pageSize);
        return await _repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash)
    {
        _logger.LogInformation("SOAP: ValidateCredentials {Email}", email);
        return await _repository.ValidateCredentialsAsync(email, passwordHash);
    }
}