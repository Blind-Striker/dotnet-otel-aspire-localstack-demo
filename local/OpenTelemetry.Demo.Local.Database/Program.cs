var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DbInitializer.ActivitySourceName));

builder.AddEventSystemDbContext();

var app = builder.Build();

await app.RunAsync();