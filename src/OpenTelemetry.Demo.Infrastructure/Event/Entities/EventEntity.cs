#pragma warning disable RCS1170, S1144

namespace OpenTelemetry.Demo.Infrastructure.Event.Entities;

public class EventEntity
{
    private List<RegistrationEntity> _registrations = [];
    private int _id;

    private EventEntity()
    {
    }

    public EventEntity(string name, DateTime date)
    {
        Name = name;
        Date = date;
    }

    public int Id => _id;
    public string Name { get; private set; }
    public DateTime Date { get; private set; }

    [NotMapped]
    public ImmutableList<RegistrationEntity>? Registrations => ImmutableList.Create(_registrations.ToArray());

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