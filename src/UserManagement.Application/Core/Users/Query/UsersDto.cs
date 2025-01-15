
namespace UserManagement.Application.Core.Users.Query;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth,
    int CountryId
);
