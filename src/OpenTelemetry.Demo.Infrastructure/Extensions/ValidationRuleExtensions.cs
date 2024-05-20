namespace OpenTelemetry.Demo.Infrastructure.Extensions;

public static class ValidationRuleExtensions
{
    public static string ToJson(this IEnumerable<ValidationFailure> failures) =>
        JsonSerializer.Serialize(new
        {
            Status = "Failed",
            Mesage = "Validation errors occurred.",
            Errors = failures.Select(f => new { Property = f.PropertyName, Error = f.ErrorMessage, }),
        });
}