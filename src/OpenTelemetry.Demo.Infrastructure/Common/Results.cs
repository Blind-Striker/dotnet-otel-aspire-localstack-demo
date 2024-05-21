#pragma warning disable MA0048
namespace OpenTelemetry.Demo.Infrastructure.Common;

public abstract record AwsFailure(string AwsResource, string Target, string Reason);

public record SnsFailure(string Reason, string Target) : AwsFailure("Sns", Target, Reason);

public record SqsFailure(string Reason, string Target) : AwsFailure("Sqs", Target, Reason);

public record ValidationFailed
{
    public ValidationFailed(IEnumerable<ValidationFailure> Errors)
    {
        this.Errors = Errors;
    }

    public ValidationFailed(ValidationFailure error) : this(new[] { error })
    {
    }

    public IEnumerable<ValidationFailure> Errors { get; init; }
}
