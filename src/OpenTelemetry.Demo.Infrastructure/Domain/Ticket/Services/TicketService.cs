namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Services;

public class TicketService(EventSystemDbContext dbContext, ILogger<TicketService> logger, IValidator<CreateTicketRequest> validator) : ITicketService
{
    private const string activitySourceName = "OpenTelemetry.Demo.Infrastructure";
    private static readonly ActivitySource ActivitySource = new(activitySourceName);

    public async Task<CreateTicketResult> CreateTicketAsync(CreateTicketRequest request)
    {
        // using Activity activity = ActivitySource.StartActivity($"{nameof(TicketService)}.{nameof(CreateTicketAsync)}")!;
        logger.LogInformation("Creating ticket with {@Request}", request);

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for {@Request}", request);

            return new ValidationFailed(validationResult.Errors);
        }

        var user = await dbContext.Users.FindAsync(request.UserId);

        if (user == null)
        {
            logger.LogWarning("User not found with {Id}", request.UserId);

            return new NotFound();
        }

        var @event = await dbContext.Events.FindAsync(request.EventId);

        if (@event == null)
        {
            logger.LogWarning("Event not found with {Id}", request.EventId);

            return new NotFound();
        }

        var ticket = new TicketEntity(user, @event, request.Status, request.Remarks);

        await dbContext.Tickets.AddAsync(ticket);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Ticket created with {Id}", ticket.Id);
        await SendEmailAsync(ticket);

        return new TicketModel(ticket.Id, ticket.UserId, ticket.EventId, ticket.Status, ticket.Remarks, ticket.CreatedAt);
    }

    private async Task SendEmailAsync(TicketEntity ticket)
    {
        // using Activity activity = ActivitySource.StartActivity($"{nameof(TicketService)}.{nameof(SendEmailAsync)}")!;
        // Simulate email sending by waiting for between 10 and 20 milliseconds
        var random = new Random();
        await Task.Delay(random.Next(10, 20));

        // Log email sending in console for demo purposes
        logger.LogInformation("Email sent to User {TicketUserId} for Event {TicketEventId}", ticket.UserId, ticket.EventId);
    }
}