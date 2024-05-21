namespace OpenTelemetry.Demo.Infrastructure.User.Models;

public record CreateUserRequest(string FirstName, string LastName, string Email);

public record UserModel(int Id, string FirstName, string LastName, string Email);