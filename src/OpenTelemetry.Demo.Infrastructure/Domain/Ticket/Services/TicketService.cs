namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Services;

public class TicketService(EventSystemDbContext dbContext, ILogger<TicketService> logger, IValidator<CreateTicketRequest> validator) : ITicketService
{
    public async Task<CreateTicketResult> CreateTicketAsync(CreateTicketRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var activity = InfrastructureActivitySource.ActivitySource.StartActivity($"{nameof(TicketService)}.{nameof(CreateTicketAsync)}")!;

        activity?.AddTag(nameof(request.UserId), request.UserId.ToString(CultureInfo.InvariantCulture));
        activity?.AddTag(nameof(request.EventId), request.EventId.ToString(CultureInfo.InvariantCulture));

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
        using var activity = InfrastructureActivitySource.ActivitySource.StartActivity($"{nameof(TicketService)}.{nameof(SendEmailAsync)}")!;

        activity?.AddTag(nameof(ticket.UserId), ticket.UserId.ToString(CultureInfo.InvariantCulture));
        activity?.AddTag(nameof(ticket.EventId), ticket.EventId.ToString(CultureInfo.InvariantCulture));

        // Simulate email sending by waiting for between 10 and 20 milliseconds
        var random = new Random();
        await Task.Delay(random.Next(10, 20));

        // Log email sending in console for demo purposes
        logger.LogInformation("Email sent to User {TicketUserId} for Event {TicketEventId}", ticket.UserId, ticket.EventId);
    }
}
