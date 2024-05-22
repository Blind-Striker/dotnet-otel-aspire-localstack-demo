namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Models;

public record CreateUserRequest(string FirstName, string LastName, string Email);

public record UserModel(int Id, string FirstName, string LastName, string Email);