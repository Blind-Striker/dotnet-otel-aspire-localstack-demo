namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Models;

[GenerateOneOf]
public partial class CreateUserResult : OneOfBase<UserModel, ValidationFailed>;

[GenerateOneOf]
public partial class GetUserResult : OneOfBase<UserModel, NotFound>;