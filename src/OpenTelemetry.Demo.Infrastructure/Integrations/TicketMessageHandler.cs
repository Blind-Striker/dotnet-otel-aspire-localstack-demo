namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public class TicketMessageHandler(ITicketService ticketService, ILogger<TicketMessageHandler> logger)
    : IMessageHandler<CreateTicketRequest>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<CreateTicketRequest> messageEnvelope, CancellationToken token = default(CancellationToken))
    {
        logger.LogInformation("Processing ticket message for {@Request}", messageEnvelope.Message);

        var createTicketResult = await ticketService.CreateTicketAsync(messageEnvelope.Message);

        return createTicketResult.Match(
            ticket =>
            {
                logger.LogInformation("Ticket created with id {TicketId}", ticket.Id);

                return MessageProcessStatus.Success();
            },
            _ =>
            {
                logger.LogError("Failed to create ticket for {@Request}", messageEnvelope.Message);

                return MessageProcessStatus.Failed();
            },
            _ =>
            {
                logger.LogError("Failed to create ticket for {@Request}", messageEnvelope.Message);

                return MessageProcessStatus.Failed();
            });
    }
}