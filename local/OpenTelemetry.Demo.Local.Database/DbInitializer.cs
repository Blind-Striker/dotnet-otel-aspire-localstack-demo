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

            await EnsureDatabaseAsync(dbContext, stoppingToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(DbContext dbContext, CancellationToken ct)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(ct).ConfigureAwait(false))
            {
                await dbCreator.CreateAsync(ct).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }
}