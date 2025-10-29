#pragma warning disable RCS1170, S1144
using System.Diagnostics.CodeAnalysis;

namespace OpenTelemetry.Demo.Infrastructure.Domain.Event.Entities;

public class RegistrationEntity
{
    private RegistrationEntity()
    {
    }

    [SetsRequiredMembers]
    public RegistrationEntity(UserEntity userEntity, EventEntity eventEntity) : this()
    {
        ArgumentNullException.ThrowIfNull(userEntity);
        ArgumentNullException.ThrowIfNull(eventEntity);

        UserId = userEntity.Id;
        EventId = eventEntity.Id;
        UserEntity = userEntity;
        EventEntity = eventEntity;
    }

    public int UserId { get; init; }
    public int EventId { get; init; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public required UserEntity UserEntity { get; set; }
    public required EventEntity EventEntity { get; set; }
}
