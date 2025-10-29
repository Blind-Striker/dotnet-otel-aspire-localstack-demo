#pragma warning disable RCS1170, S1144
using System.Diagnostics.CodeAnalysis;

namespace OpenTelemetry.Demo.Infrastructure.Domain.Event.Entities;

public class EventEntity
{
    private readonly List<RegistrationEntity> _registrations;
    private readonly List<TicketEntity> _tickets;

    private EventEntity()
    {
        _registrations = [];
        _tickets = [];
    }

    [SetsRequiredMembers]
    public EventEntity(string name, DateTime date)
        : this()
    {
        Name = name;
        Date = date;
    }

    public int Id { get; init; }

    public required string Name { get; set; }
    public DateTime Date { get; set; }

    public IReadOnlyCollection<RegistrationEntity> Registrations => _registrations;
    public IReadOnlyCollection<TicketEntity> Tickets => _tickets;

    public void AddRegistration(UserEntity userEntity)
    {
        if (_registrations.Exists(r => r.UserId == userEntity.Id))
        {
            throw new InvalidOperationException("User is already registered for this event.");
        }

        var registration = new RegistrationEntity(userEntity, this);
        _registrations.Add(registration);
    }
}
