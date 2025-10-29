namespace OpenTelemetry.Demo.Infrastructure.Common;

public record ValidationFailed
{
    public ValidationFailed(IEnumerable<ValidationFailure> Errors)
    {
        this.Errors = Errors;
    }

    public ValidationFailed(ValidationFailure error) : this([error])
    {
    }

    public IEnumerable<ValidationFailure> Errors { get; init; }
}