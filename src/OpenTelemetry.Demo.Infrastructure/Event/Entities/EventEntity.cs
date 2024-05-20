#pragma warning disable RCS1170, S1144

namespace OpenTelemetry.Demo.Infrastructure.Event.Entities;

public class EventEntity
{
    private List<RegistrationEntity> _registrations;

    private EventEntity()
    {
        _registrations = new List<RegistrationEntity>();
    }

    public EventEntity(string name, DateTime date)
        : this()
    {
        Name = name;
        Date = date;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
    public DateTime Date { get; private set; }

    public IReadOnlyCollection<RegistrationEntity> Registrations => _registrations;

    public void AddRegistration(UserEntity userEntity)
    {
        _registrations ??= [];

        if (_registrations.Exists(r => r.UserId == userEntity.Id))
        {
            throw new InvalidOperationException("User is already registered for this event.");
        }

        var registration = new RegistrationEntity(userEntity, this);
        _registrations.Add(registration);
    }
}