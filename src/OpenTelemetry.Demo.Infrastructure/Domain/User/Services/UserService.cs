namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Services;

public class UserService(EventSystemDbContext dbContext, ILogger<UserService> logger, IValidator<CreateUserRequest> validator) : IUserService
{
    public async Task<CreateUserResult> CreateUserAsync(CreateUserRequest request)
    {
        logger.LogInformation("Creating user with {@Request}", request);

        ValidationResult? validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {@Request}", request);
            return new ValidationFailed(validationResult.Errors);
        }

        var user = new UserEntity(request.FirstName, request.LastName, request.Email);

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("User created with {Id}", user.Id);

        return new UserModel(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<GetUserResult> GetUserByIdAsync(int id)
    {
        logger.LogInformation("Getting user with {Id}", id);

        UserEntity? user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            logger.LogWarning("User not found with {Id}", id);
            return new NotFound();
        }

        logger.LogInformation("User found with {Id}", id);

        return new UserModel(user.Id, user.FirstName, user.LastName, user.Email);
    }
}