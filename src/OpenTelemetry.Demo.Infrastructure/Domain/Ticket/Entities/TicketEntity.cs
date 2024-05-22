namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Entities;

public class TicketEntity
{
    private TicketEntity()
    {
    }

    public TicketEntity(UserEntity userEntity, EventEntity eventEntity, string status = "Reserved", string? remarks = null) : this()
    {
        UserId = userEntity.Id;
        EventId = eventEntity.Id;
        Status = status;
        Remarks = remarks;
        User = userEntity;
        Event = eventEntity;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int EventId { get; private set; }
    public string Status { get; private set; }
    public string? Remarks { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public UserEntity User { get; private set; }
    public EventEntity Event { get; private set; }
}