namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Interfaces;

public interface ITicketService
{
    Task<CreateTicketResult> CreateTicketAsync(CreateTicketRequest request);
}