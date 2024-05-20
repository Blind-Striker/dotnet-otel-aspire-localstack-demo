#pragma warning disable RCS1170, S1144

namespace OpenTelemetry.Demo.Infrastructure.User.Entities;

public class UserEntity
{
    private List<RegistrationEntity> _registrations = [];
    private int _id;

    private UserEntity()
    {
    }

    public UserEntity(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public int Id => _id;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }

    [NotMapped]
    public ImmutableList<RegistrationEntity>? Registrations => ImmutableList.Create(_registrations.ToArray());
}