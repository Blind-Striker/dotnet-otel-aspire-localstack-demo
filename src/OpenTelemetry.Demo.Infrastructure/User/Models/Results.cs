namespace OpenTelemetry.Demo.Infrastructure.User.Models;

[GenerateOneOf]
public partial class CreateUserResult : OneOfBase<UserModel, ValidationFailed>;

[GenerateOneOf]
public partial class GetUserResult : OneOfBase<UserModel, NotFound>;