using System.ServiceModel;
using SmartCity.DataLayer.UserService.DataContracts;

namespace SmartCity.DataLayer.UserService.Contracts;

[ServiceContract(Namespace = "http://smartcity.transport/userdata/v1")]
public interface IUserDataService
{
    [OperationContract]
    Task<UserDataContract?> GetUserByIdAsync(Guid userId);

    [OperationContract]
    Task<UserDataContract?> GetUserByEmailAsync(string email);

    [OperationContract]
    Task<Guid> CreateUserAsync(CreateUserDataContract userData);

    [OperationContract]
    Task<bool> UpdateUserAsync(UserDataContract userData);

    [OperationContract]
    Task<bool> DeleteUserAsync(Guid userId);

    [OperationContract]
    Task<UserDataContract[]> GetAllUsersAsync(int pageNumber, int pageSize);

    [OperationContract]
    Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash);
}