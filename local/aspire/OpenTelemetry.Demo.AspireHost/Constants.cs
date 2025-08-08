namespace OpenTelemetry.Demo.AspireHost;

public static class Constants
{
    public const string DatabaseName = "eventdb";
    public const string DatabaseTypeKey = "EventSystem:DatabaseType";
    public const string TicketIntegrationKey = "EventSystem:TicketIntegration";

    public const string SqlType = "sqlserver";
    public const string NpgsqlType = "npgsql";
    public const string AwsType = "aws";
    public const string HttpType = "http";

    public const string DefaultDatabaseType = NpgsqlType;
    public const string DefaultTicketIntegration = AwsType;
}
