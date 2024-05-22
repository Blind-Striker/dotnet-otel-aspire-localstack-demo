#pragma warning disable MA0048
namespace OpenTelemetry.Demo.Infrastructure.Integrations;

public abstract record IntegrationFailure(string Resource, string Target, string Reason);

public record HttpFailure(string Reason, string Target) : IntegrationFailure("Http", Reason, Target);

public record SnsFailure(string Reason, string Target) : IntegrationFailure("Sns", Reason, Target);

public record SqsFailure(string Reason, string Target) : IntegrationFailure("Sqs", Reason, Target);

[GenerateOneOf]
public partial class TicketBookingResult : OneOfBase<TicketModel, ValidationFailed, IntegrationFailure>;