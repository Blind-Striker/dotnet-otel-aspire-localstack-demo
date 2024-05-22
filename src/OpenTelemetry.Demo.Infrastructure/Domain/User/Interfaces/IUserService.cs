namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Interfaces;

public interface IUserService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserRequest request);

    Task<GetUserResult> GetUserByIdAsync(int id);
}