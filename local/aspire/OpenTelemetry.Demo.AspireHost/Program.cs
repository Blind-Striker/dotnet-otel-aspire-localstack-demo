const string databaseName = "eventdb";
const string databaseTypeEnv = "EventSystem:DatabaseType";
const string ticketEnv = "EventSystem:TicketIntegration";

// var databaseType = Environment.GetEnvironmentVariable(databaseTypeEnv) ?? "npgsql";
var databaseType = "npgsql"; // it seems like the env variables in launchSettings.json are not being read
var ticketIntegration = "aws";
var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<IResourceWithConnectionString> database = databaseType switch
{
    "sqlserver" => builder.AddSqlServer("sql").WithHealthCheck().AddDatabase(databaseName),
    "npgsql" => builder.AddPostgres("postgres").WithHealthCheck().WithEnvironment("POSTGRES_DB", databaseName).AddDatabase(databaseName),
    _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
};

var awsConfig = builder.AddAWSSDKConfig()
                       .WithProfile("default")
                       .WithRegion(RegionEndpoint.USWest2);

var localStackOptions = builder.AddLocalStackConfig();

var localStack = builder.AddLocalStack("localstack", localStackOptions);

var awsResources = builder.AddAWSCloudFormationTemplate("CustomerFeedbackAppResources", "aws-resources.template")
                          .WithReference(awsConfig)
                          .WithLocalStack(localStackOptions);

var databaseInitializer = builder
                          .AddProject<Projects.OpenTelemetry_Demo_Local_Database>("db-initializer")
                          .WithReference(database)
                          .WithEnvironment(databaseTypeEnv, databaseType);

//.WaitFor(database);

var ticketApi = builder
                .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
                .WithExternalHttpEndpoints()
                .WithReference(database)
                .WithReference(awsResources)
                .WithReference(localStack)
                .WithEnvironment(databaseTypeEnv, databaseType)
                .WithEnvironment(ticketEnv, ticketIntegration);

//.WaitFor(localStack)
//.WaitFor(database);

var eventApi = builder
               .AddProject<Projects.OpenTelemetry_Demo_EventApi>("event-api")
               .WithExternalHttpEndpoints()
               .WithReference(ticketApi)
               .WithReference(database)
               .WithReference(awsResources)
               .WithReference(localStack)
               .WithEnvironment(databaseTypeEnv, databaseType)
               .WithEnvironment(ticketEnv, ticketIntegration);

//.WaitFor(localStack)
//.WaitFor(database);

var ticketProcessor = builder
                      .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor")
                      .WithReference(database)
                      .WithReference(awsResources)
                      .WithReference(localStack)
                      .WithEnvironment(databaseTypeEnv, databaseType);

await builder.Build().RunAsync();