// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Hosting;

public static class HostExtensions
{
    public static HostApplicationBuilder AddEventSystemDbContext(this HostApplicationBuilder builder, string connStrKey = "eventdb")
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