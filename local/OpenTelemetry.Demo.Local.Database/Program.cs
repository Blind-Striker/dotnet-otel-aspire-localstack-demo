HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DbInitializer>();

builder.AddServiceDefaults();
builder.AddEventSystemDbContextPool();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DbInitializer.ActivitySourceName));

IHost app = builder.Build();

await app.RunAsync();