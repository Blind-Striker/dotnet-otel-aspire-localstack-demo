#pragma warning disable RCS1170, S1144
namespace OpenTelemetry.Demo.Infrastructure.User.Entities;

public class UserEntity
{
    private readonly List<RegistrationEntity> _registrations;

    private UserEntity()
    {
        _registrations = new List<RegistrationEntity>();
    }

    public UserEntity(string firstName, string lastName, string email)
        : this()
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }

    public IReadOnlyCollection<RegistrationEntity> Registrations => _registrations;
}