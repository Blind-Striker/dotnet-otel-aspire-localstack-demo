namespace OpenTelemetry.Demo.Infrastructure.Domain.Event.Services;

public class EventService(
    EventSystemDbContext dbContext,
    ITicketBookingClient ticketBookingClient,
    ILogger<EventService> logger,
    IValidator<RegisterToEventRequest> validator) : IEventService
{
    private const string activitySourceName = "OpenTelemetry.Demo.Infrastructure";
    private static readonly ActivitySource ActivitySource = new(activitySourceName);

    public async Task<GetEventResult> GetEventByIdAsync(int id)
    {
        logger.LogInformation("Getting event with {Id}", id);

        var @event = await dbContext.Events.FindAsync(id);

        if (@event == null)
        {
            logger.LogWarning("Event not found with {Id}", id);

            return new NotFound();
        }

        logger.LogInformation("Event found with {Id}", id);

        return new EventModel(@event.Id, @event.Name, @event.Date);
    }

    public async Task<GetEventsResult> GetEventsAsync()
    {
        logger.LogInformation("Getting all events");

        var events = await dbContext.Events.ToListAsync();

        logger.LogInformation("Found {Count} events", events.Count);

        return events.Select(@event => new EventModel(@event.Id, @event.Name, @event.Date)).ToArray();
    }

    public async Task<RegisterToEventResult> RegisterToEventAsync(RegisterToEventRequest request)
    {
        // using Activity activity = ActivitySource.StartActivity($"{nameof(EventService)}.{nameof(RegisterToEventAsync)}")!;

        logger.LogInformation("Registering user to event with {@Request}", request);

        ValidationResult? validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {@Request}", request);

            return new ValidationFailed(validationResult.Errors);
        }

        UserEntity? user = await dbContext.Users.FindAsync(request.UserId);

        if (user == null)
        {
            logger.LogWarning("User not found with {Id}", request.UserId);

            return new NotFound();
        }

        EventEntity? @event = await dbContext.Events.FindAsync(request.EventId);

        if (@event == null)
        {
            logger.LogWarning("Event not found with {Id}", request.EventId);

            return new NotFound();
        }

        RegistrationEntity? userEvent = await dbContext.Registrations.SingleOrDefaultAsync(entity => entity.UserId == user.Id && entity.EventId == @event.Id);

        if (userEvent != null)
        {
            logger.LogWarning("User already registered to event with {UserId} and {EventId}", user.Id, @event.Id);
        }
        else
        {
            userEvent = new RegistrationEntity(user, @event);

            await dbContext.Registrations.AddAsync(userEvent);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("User registered to event with {UserId} and {EventId}", user.Id, @event.Id);
        }

        TicketBookingResult ticketBookingResult = await ticketBookingClient
                                                      .CreateTicketAsync(new CreateTicketRequest(user.Id, @event.Id, "Pending", "Registered to event"));

        if (ticketBookingResult.IsT1 || ticketBookingResult.IsT2) // ValidationFailed or IntegrationFailed
        {
            logger.LogError("Failed to create ticket for user {UserId} and event {EventId}", user.Id, @event.Id);
        }

        return new UserEventModel(user.Id, user.FirstName, user.LastName, @event.Id, @event.Name, @event.Date);
    }
}