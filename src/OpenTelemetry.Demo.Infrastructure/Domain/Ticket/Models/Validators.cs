namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Models;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}