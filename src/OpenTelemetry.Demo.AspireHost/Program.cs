var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddProject<Projects.OpenTelemetry_Demo_UserApi>("UserApi")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync().ConfigureAwait(false);