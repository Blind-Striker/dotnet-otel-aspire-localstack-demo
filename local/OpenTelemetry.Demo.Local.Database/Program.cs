var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DbInitializer>();
builder.Services.AddHostedService<DbSeeder>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DbInitializer.ActivitySourceName))
    .WithTracing(tracing => tracing.AddSource(DbSeeder.ActivitySourceName));

builder.Services.AddDbContextPool<EventSystemDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("eventdb");
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.ExecutionStrategy(c => new SqlServerRetryingExecutionStrategy(c)));
});

builder.EnrichSqlServerDbContext<EventSystemDbContext>(settings => settings.DisableRetry = true);

var app = builder.Build();

await app.RunAsync().ConfigureAwait(false);