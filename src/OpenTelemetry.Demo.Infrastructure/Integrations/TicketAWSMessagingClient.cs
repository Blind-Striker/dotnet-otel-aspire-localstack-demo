namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public class TicketAWSMessagingClient(IMessagePublisher messagePublisher, ILogger<TicketAWSMessagingClient> logger, IValidator<CreateTicketRequest> validator)
    : ITicketBookingClient
{
    private const string activitySourceName = "OpenTelemetry.Demo.Infrastructure";
    private static readonly ActivitySource ActivitySource = new(activitySourceName);

    public async Task<TicketBookingResult> CreateTicketAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        // using Activity activity = ActivitySource.StartActivity($"{nameof(TicketAWSMessagingClient)}.{nameof(CreateTicketAsync)}")!;
        logger.LogInformation("Creating ticket for user {UserId} and event {EventId}", request.UserId, request.EventId);

        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            logger.LogError("Validation failed for request {@Request}", request);

            return new ValidationFailed(validationResult.Errors);
        }

        try
        {
            await messagePublisher.PublishAsync(request, ct);
        }
        catch (AmazonServiceException e)
        {
            logger.LogError(e, "Failed to publish message to SNS");

            return new SnsFailure(e.Message, "SNS");
        }

        return new None();
    }
}