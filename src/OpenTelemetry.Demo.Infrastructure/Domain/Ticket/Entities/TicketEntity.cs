using System.Diagnostics.CodeAnalysis;

namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Entities;

public class TicketEntity
{
    private TicketEntity()
    {
    }

    [SetsRequiredMembers]
    public TicketEntity(UserEntity userEntity, EventEntity eventEntity, string status = "Reserved", string? remarks = null) : this()
    {
        ArgumentNullException.ThrowIfNull(userEntity);
        ArgumentNullException.ThrowIfNull(eventEntity);

        UserId = userEntity.Id;
        EventId = eventEntity.Id;
        Status = status;
        Remarks = remarks;
        User = userEntity;
        Event = eventEntity;
    }

    public int Id { get; init; }
    public int UserId { get; init; }
    public int EventId { get; init; }
    public required string Status { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required UserEntity User { get; set; }
    public required EventEntity Event { get; set; }
}
