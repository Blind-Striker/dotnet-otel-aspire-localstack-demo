namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public class TicketMessageHandler(ITicketService ticketService, ILogger<TicketMessageHandler> logger)
    : IMessageHandler<CreateTicketRequest>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<CreateTicketRequest> messageEnvelope, CancellationToken token = default(CancellationToken))
    {
        logger.LogInformation("Processing ticket message for {@Request}", messageEnvelope.Message);

        CreateTicketResult createTicketResult = await ticketService.CreateTicketAsync(messageEnvelope.Message);

        return createTicketResult.Match(
            ticket =>
            {
                logger.LogInformation("Ticket created with id {TicketId}", ticket.Id);

                return MessageProcessStatus.Success();
            },
            validationFailed =>
            {
                logger.LogError("Failed to create ticket for {@Request}", messageEnvelope.Message);

                return MessageProcessStatus.Failed();
            },
            notFound =>
            {
                logger.LogError("Failed to create ticket for {@Request}", messageEnvelope.Message);

                return MessageProcessStatus.Failed();
            });
    }
}