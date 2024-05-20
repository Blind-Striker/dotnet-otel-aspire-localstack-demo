#pragma warning disable RCS1170, S1144

namespace OpenTelemetry.Demo.Infrastructure.Event.Entities;

public class RegistrationEntity
{
    private RegistrationEntity()
    {
    }

    public RegistrationEntity(UserEntity userEntity, EventEntity eventEntity) : this()
    {
        UserId = userEntity.Id;
        EventId = eventEntity.Id;
        UserEntity = userEntity;
        EventEntity = eventEntity;
    }

    public int UserId { get; private set; }
    public int EventId { get; private set; }
    public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;
    public UserEntity UserEntity { get; private set; }
    public EventEntity EventEntity { get; private set; }
}