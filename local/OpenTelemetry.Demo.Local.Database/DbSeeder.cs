namespace OpenTelemetry.Demo.Local.Database;

public sealed class DbSeeder(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Seeders";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Initialize database", ActivityKind.Client);

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EventSystemDbContext>();

            await SeedDataAsync(dbContext);
        }
        catch (Exception e)
        {
            activity?.RecordException(e);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task SeedDataAsync(EventSystemDbContext dbContext)
    {
        // Seed Event Data
        await dbContext.Events.AddAsync(new EventEntity(".NET Conf 2024", DateTime.UtcNow.AddDays(4))).ConfigureAwait(false);
        await dbContext.Events.AddAsync(new EventEntity("Azure DevOps Summit", DateTime.UtcNow.AddDays(3))).ConfigureAwait(false);
        await dbContext.Events.AddAsync(new EventEntity("Serverless Turkey", DateTime.UtcNow.AddDays(4))).ConfigureAwait(false);
        await dbContext.Events.AddAsync(new EventEntity("Codefiction Meetup", DateTime.UtcNow.AddDays(5))).ConfigureAwait(false);

        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}