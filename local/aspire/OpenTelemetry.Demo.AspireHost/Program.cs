const string databaseName = "eventdb";
const string databaseTypeEnv = "EventSystem:DatabaseType";
const string ticketEnv = "EventSystem:TicketIntegration";

// var databaseType = Environment.GetEnvironmentVariable(databaseTypeEnv) ?? "npgsql";
const string databaseType = "npgsql"; // it seems like the env variables in launchSettings.json are not being read
const string ticketIntegration = "aws";
var awsResourcesTemplate = Path.Combine(Directory.GetCurrentDirectory(), "aws-resources.template");

if (!File.Exists(awsResourcesTemplate))
{
    throw new ArgumentException($"Could not find `{awsResourcesTemplate}` template.");
}

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<IResourceWithConnectionString> database = databaseType switch
{
    "sqlserver" => builder.AddSqlServer("sql")
                          .AddDatabase(databaseName),
    "npgsql" => builder.AddPostgres("postgres")
                       .WithEnvironment("POSTGRES_DB", databaseName)
                       .AddDatabase(databaseName),
    _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}"),
};

var localStackOptions = builder.AddLocalStackConfig();
var localStack = builder.AddLocalStack("localstack", localStackOptions);

var awsResources = builder
                   .AddAWSCloudFormationTemplate("CustomerFeedbackAppResources", awsResourcesTemplate)
                   .WaitFor(localStack)
                   .WithLocalStack(localStackOptions);

builder
    .AddProject<Projects.OpenTelemetry_Demo_Local_Database>("db-initializer")
    .WaitFor(database)
    .WithReference(database)
    .WithEnvironment(databaseTypeEnv, databaseType);

var ticketApi = builder
                .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
                .WithExternalHttpEndpoints()
                .WaitFor(database)
                .WaitFor(localStack)
                .WithReference(database)
                .WithReference(awsResources)
                .WithReference(localStack)
                .WithEnvironment(databaseTypeEnv, databaseType)
                .WithEnvironment(ticketEnv, ticketIntegration);

builder
    .AddProject<Projects.OpenTelemetry_Demo_EventApi>("event-api")
    .WithExternalHttpEndpoints()
    .WaitFor(database)
    .WaitFor(localStack)
    .WithReference(ticketApi)
    .WithReference(database)
    .WithReference(awsResources)
    .WithReference(localStack)
    .WithEnvironment(databaseTypeEnv, databaseType)
    .WithEnvironment(ticketEnv, ticketIntegration);

builder
    .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor")
    .WaitFor(database)
    .WaitFor(localStack)
    .WithReference(database)
    .WithReference(awsResources)
    .WithReference(localStack)
    .WithEnvironment(databaseTypeEnv, databaseType);

await builder.Build().RunAsync();