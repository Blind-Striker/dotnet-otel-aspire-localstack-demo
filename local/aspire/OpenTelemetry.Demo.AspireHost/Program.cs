var builder = DistributedApplication.CreateBuilder(args);

var databaseType = builder.Configuration.GetSection(Constants.DatabaseTypeKey).Value ?? Constants.DefaultDatabaseType;
var ticketIntegration = builder.Configuration.GetSection(Constants.TicketIntegrationKey).Value ?? Constants.DefaultTicketIntegration;

// Set up a configuration for the AWS .NET SDK
var awsConfig = builder.AddAWSSDKConfig().WithRegion(RegionEndpoint.EUCentral1);

// Bootstrap the localstack container with enhanced configuration
var localstack = builder
    .AddLocalStack(awsConfig: awsConfig, configureContainer: container =>
    {
        container.Lifetime = ContainerLifetime.Session;
        container.DebugLevel = 1;
        container.LogLevel = LocalStackLogLevel.Debug;
    });

var eventSystemStack = builder
    .AddAWSCDKStack("event-system-stack", scope => new EventSystemStack(scope, "event-system-construct"))
    .WithReference(awsConfig);

eventSystemStack.AddOutput("TicketQueueUrl", stack => stack.TicketQueue.QueueUrl);
eventSystemStack.AddOutput("TicketTopicArn", stack => stack.TicketTopic.TopicArn);

IResourceBuilder<IResourceWithConnectionString> database = databaseType switch
{
    Constants.SqlType => builder.AddSqlServer("sql").AddDatabase(Constants.DatabaseName),
    Constants.NpgsqlType => builder.AddPostgres("postgres").WithEnvironment("POSTGRES_DB", Constants.DatabaseName).AddDatabase(Constants.DatabaseName),
    _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}"),
};

builder
    .AddProject<Projects.OpenTelemetry_Demo_Local_Database>("db-initializer")
    .WaitFor(database)
    .WithReference(database)
    .WithEnvironment(Constants.DatabaseTypeKey, databaseType);

IResourceBuilder<ProjectResource>? ticketApi = null;
if (string.Equals(ticketIntegration, Constants.HttpType, StringComparison.Ordinal))
{
    ticketApi = builder
        .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
        .WithExternalHttpEndpoints()
        .WithReference(eventSystemStack)
        .WaitFor(database)
        .WithReference(database)
        .WithEnvironment(Constants.DatabaseTypeKey, databaseType)
        .WithEnvironment(Constants.TicketIntegrationKey, ticketIntegration);
}

var eventApi = builder
    .AddProject<Projects.OpenTelemetry_Demo_EventApi>("event-api")
    .WithExternalHttpEndpoints()
    .WithReference(eventSystemStack)
    .WaitFor(database)
    .WithReference(database)
    .WithEnvironment(Constants.DatabaseTypeKey, databaseType)
    .WithEnvironment(Constants.TicketIntegrationKey, ticketIntegration);

if (string.Equals(ticketIntegration, Constants.HttpType, StringComparison.Ordinal) && ticketApi != null)
{
    eventApi.WithReference(ticketApi);
}

if (string.Equals(ticketIntegration, Constants.AwsType, StringComparison.Ordinal))
{
    builder
        .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor")
        .WithReference(eventSystemStack)
        .WaitFor(database)
        .WithReference(database)
        .WithEnvironment(Constants.DatabaseTypeKey, databaseType);
}

builder.UseLocalStack(localstack);

await builder.Build().RunAsync();
