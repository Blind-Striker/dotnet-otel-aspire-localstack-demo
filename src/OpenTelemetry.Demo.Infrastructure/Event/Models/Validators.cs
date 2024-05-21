namespace OpenTelemetry.Demo.Infrastructure.Event.Models;

public class RegisterToEventRequestValidator : AbstractValidator<RegisterToEventRequest>
{
    public RegisterToEventRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EventId).NotEmpty();
    }
}