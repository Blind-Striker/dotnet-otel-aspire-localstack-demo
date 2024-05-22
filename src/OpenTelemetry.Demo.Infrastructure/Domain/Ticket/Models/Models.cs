namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Models;

public record CreateTicketRequest(int UserId, int EventId, string Status, string? Remarks);

public record TicketModel(int Id, int UserId, int EventId, string Status, string? Remarks, DateTime CreatedAt);