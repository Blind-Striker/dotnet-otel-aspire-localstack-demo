namespace OpenTelemetry.Demo.Infrastructure;

public static class InfrastructureActivitySource
{
    public const string ActivitySourceName = "OpenTelemetry.Demo.Infrastructure";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
