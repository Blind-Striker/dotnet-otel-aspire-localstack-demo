namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public interface ITicketBookingClient
{
    Task<TicketBookingResult> CreateTicketAsync(CreateTicketRequest request, CancellationToken ct = default);
}