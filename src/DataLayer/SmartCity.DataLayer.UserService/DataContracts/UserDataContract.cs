using System.Runtime.Serialization;

namespace SmartCity.DataLayer.UserService.DataContracts;

[DataContract(Namespace = "http://smartcity.transport/userdata/v1")]
public class UserDataContract
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string? PhoneNumber { get; set; }

    [DataMember(Order = 6)]
    public DateTime? DateOfBirth { get; set; }

    [DataMember(Order = 7)]
    public string? ProfilePictureUrl { get; set; }

    [DataMember(Order = 8)]
    public bool EmailVerified { get; set; }

    [DataMember(Order = 9)]
    public string AccountStatus { get; set; } = "active";

    [DataMember(Order = 10)]
    public string UserType { get; set; } = "regular";

    [DataMember(Order = 11)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 12)]
    public DateTime UpdatedAt { get; set; }
}

[DataContract(Namespace = "http://smartcity.transport/userdata/v1")]
public class CreateUserDataContract
{
    [DataMember(Order = 1, IsRequired = true)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 2, IsRequired = true)]
    public string PasswordHash { get; set; } = string.Empty;

    [DataMember(Order = 3, IsRequired = true)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(Order = 4, IsRequired = true)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string? PhoneNumber { get; set; }

    [DataMember(Order = 6)]
    public DateTime? DateOfBirth { get; set; }
}