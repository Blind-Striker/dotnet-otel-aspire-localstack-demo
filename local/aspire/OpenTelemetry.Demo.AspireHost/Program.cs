using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

const string databaseName = "eventdb";
const string databaseTypeEnv = "EventSystem:DatabaseType";

var databaseType = Environment.GetEnvironmentVariable(databaseTypeEnv) ?? "npgsql";

IResourceBuilder<IResourceWithConnectionString> database = databaseType switch
{
    "sqlserver" => builder.AddSqlServer("sql").WithHealthCheck().AddDatabase(databaseName),
    "npgsql" => builder.AddPostgres("postgres").WithHealthCheck().WithEnvironment("POSTGRES_DB", databaseName).AddDatabase(databaseName),
    _ => throw new InvalidOperationException($"Unsupported database type: {databaseType}")
};

var databaseInitializer = builder
                          .AddProject<Projects.OpenTelemetry_Demo_Local_Database>("db-initializer")
                          .WithReference(database)
                          .WithEnvironment(databaseTypeEnv, databaseType)
                          .WaitFor(database);

// var ticketApi = builder
//     .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
//     .WithReference(database)
//     .WithEnvironment(databaseTypeEnv, databaseType)
//     .WaitFor(database);

var userApi = builder
              .AddProject<Projects.OpenTelemetry_Demo_EventApi>("user-api")
              .WithExternalHttpEndpoints()

              // .WithReference(ticketApi)
              .WithReference(database)
              .WithEnvironment(databaseTypeEnv, databaseType)
              .WaitFor(database);

// var ticketProcessor = builder
//     .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor")
//     .WithReference(database)
//     .WithEnvironment(databaseTypeEnv, databaseType)
//     .WaitFor(database);

await builder.Build().RunAsync();