var builder = DistributedApplication.CreateBuilder(args);

var sqlResource = builder.AddSqlServer("sql");
var eventDbResource = sqlResource.AddDatabase("eventdb");

var databaseInitailizerResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_Local_Database>("db-initializer")
    .WithReference(eventDbResource);

var ticketApiResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
    .WithReference(eventDbResource);

var userApiResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_UserApi>("user-api")
    .WithExternalHttpEndpoints()
    .WithReference(ticketApiResource)
    .WithReference(eventDbResource);

var ticketProcessorResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor")
    .WithReference(eventDbResource);

await builder.Build().RunAsync().ConfigureAwait(false);