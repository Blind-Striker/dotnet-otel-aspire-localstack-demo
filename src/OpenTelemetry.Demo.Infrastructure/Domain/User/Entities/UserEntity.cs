#pragma warning disable RCS1170, S1144
namespace OpenTelemetry.Demo.Infrastructure.Domain.User.Entities;

public class UserEntity
{
    private readonly List<RegistrationEntity> _registrations;
    private readonly List<TicketEntity> _tickets;

    private UserEntity()
    {
        _registrations = new List<RegistrationEntity>();
        _tickets = new List<TicketEntity>();
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
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<RegistrationEntity> Registrations => _registrations;
    public IReadOnlyCollection<TicketEntity> Tickets => _tickets;
}