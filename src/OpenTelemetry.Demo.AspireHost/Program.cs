var builder = DistributedApplication.CreateBuilder(args);

var userApiResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_UserApi>("user-api")
    .WithExternalHttpEndpoints();

var ticketApiResource = builder
    .AddProject<Projects.OpenTelemetry_Demo_TicketApi>("ticket-api")
    .WithReference(userApiResource);

builder
    .AddProject<Projects.OpenTelemetry_Demo_TicketProcessor>("ticket-processor");

await builder.Build().RunAsync().ConfigureAwait(false);