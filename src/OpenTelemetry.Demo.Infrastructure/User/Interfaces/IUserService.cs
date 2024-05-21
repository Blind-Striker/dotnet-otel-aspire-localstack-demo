namespace OpenTelemetry.Demo.Infrastructure.User.Interfaces;

public interface IUserService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserRequest request);

    Task<GetUserResult> GetUserByIdAsync(int id);
}