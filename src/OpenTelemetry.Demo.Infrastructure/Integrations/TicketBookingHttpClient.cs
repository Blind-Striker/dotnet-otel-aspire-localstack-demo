namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public class TicketBookingHttpClient(HttpClient httpClient, ILogger<TicketBookingHttpClient> logger, IValidator<CreateTicketRequest> validator)
    : ITicketBookingClient
{
    private const string activitySourceName = "OpenTelemetry.Demo.Infrastructure";
    private static readonly ActivitySource ActivitySource = new(activitySourceName);

    public async Task<TicketBookingResult> CreateTicketAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        // using Activity activity = ActivitySource.StartActivity($"{nameof(TicketBookingHttpClient)}.{nameof(CreateTicketAsync)}")!;
        logger.LogInformation("Creating ticket for user {UserId} and event {EventId}", request.UserId, request.EventId);

        ValidationResult? validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            logger.LogError("Validation failed for request {@Request}", request);

            return new ValidationFailed(validationResult.Errors);
        }

        var jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = EventSystemJsonSerializerContext.Default };
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("ticket", request, jsonSerializerOptions, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to create ticket for user {UserId} and event {EventId}", request.UserId, request.EventId);

            return new HttpFailure(response.StatusCode.ToString(), response.ReasonPhrase ?? "Unknown error");
        }

        var ticket = await response.Content.ReadFromJsonAsync<TicketModel>(jsonSerializerOptions, ct);

        return ticket;
    }
}