namespace OpenTelemetry.Demo.Infrastructure.Domain.Ticket.Models;

[GenerateOneOf]
public partial class CreateTicketResult : OneOfBase<TicketModel, ValidationFailed, NotFound>;