// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class DbContextExtensions
{
    public static IHostApplicationBuilder AddEventSystemDbContext(this IHostApplicationBuilder builder, string connStrKey = "eventdb",
                                                                  Action<MicrosoftEntityFrameworkCoreSqlServerSettings>? configureSqlSettings = null,
                                                                  Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>? configureNpgsqlSettings = null,
                                                                  Action<DbContextOptionsBuilder>? configureDbContextOptions = null)
    {
        connStrKey = string.IsNullOrWhiteSpace(connStrKey) ? "eventdb" : connStrKey;
        var databaseType = builder.Configuration.GetValue<string>("EventSystem:DatabaseType") ?? "npgsql";

        switch (databaseType)
        {
            case "sqlserver":
                builder.AddSqlServerDbContext<EventSystemDbContext>(connStrKey, configureSqlSettings, configureDbContextOptions);
                break;
            case "npgsql":
                builder.AddNpgsqlDbContext<EventSystemDbContext>(connStrKey, configureNpgsqlSettings, configureDbContextOptions);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database type: {databaseType}");
        }
        return builder;
    }

    public static IHostApplicationBuilder AddEventSystemDbContextPool(this IHostApplicationBuilder builder, string connStrKey = "eventdb")
    {
        connStrKey = string.IsNullOrWhiteSpace(connStrKey) ? "eventdb" : connStrKey;
        var connectionString = builder.Configuration.GetConnectionString(connStrKey);
        var databaseType = builder.Configuration.GetValue<string>("EventSystem:DatabaseType") ?? "npgsql";

        builder.Services.AddDbContextPool<EventSystemDbContext>(options =>
        {
            switch (databaseType)
            {
                case "sqlserver":
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        // Workround for https://github.com/dotnet/aspire/issues/1023
                        sqlOptions.ExecutionStrategy(c => new SqlServerRetryingExecutionStrategy(c));
                    });

                    break;
                case "npgsql":
                    options.UseNpgsql(connectionString, postgresOptions => postgresOptions.ExecutionStrategy(c => new NpgsqlRetryingExecutionStrategy(c)));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported database type: {databaseType}");
            }
        });

        builder.EnrichSqlServerDbContext<EventSystemDbContext>(settings =>

                                                                   // Disable Aspire default retries as we're using a custom execution strategy
                                                                   settings.DisableRetry = true);

        return builder;
    }
}