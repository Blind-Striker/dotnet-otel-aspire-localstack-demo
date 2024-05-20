namespace OpenTelemetry.Demo.Local.Database;

public sealed class DbInitializer(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Initializations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Initialize database", ActivityKind.Client);

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EventSystemDbContext>();
            await EnsureDatabaseAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(EventSystemDbContext dbContext, CancellationToken ct)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var dbCreated = await dbContext.Database.EnsureCreatedAsync(ct);

            if (dbCreated)
            {
                await SeedDataAsync(dbContext, ct);
            }
        });
    }

    private static async Task SeedDataAsync(EventSystemDbContext dbContext, CancellationToken ct)
    {
        // Seed Event Data
        await dbContext.Events.AddAsync(new EventEntity(".NET Conf 2024", DateTime.UtcNow.AddDays(4)), ct);
        await dbContext.Events.AddAsync(new EventEntity("Azure DevOps Summit", DateTime.UtcNow.AddDays(3)), ct);
        await dbContext.Events.AddAsync(new EventEntity("Serverless Turkey", DateTime.UtcNow.AddDays(4)), ct);
        await dbContext.Events.AddAsync(new EventEntity("Codefiction Meetup", DateTime.UtcNow.AddDays(5)), ct);

        await dbContext.SaveChangesAsync(ct);
    }
}