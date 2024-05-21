#pragma warning disable CA1305

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetryLogging();
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        var config = new LoggerConfiguration()
                     .ReadFrom.Configuration(builder.Configuration)
                     .Enrich.FromLogContext()
                     .Enrich.WithMachineName()
                     .Enrich.WithProcessId()
                     .Enrich.WithProcessName()
                     .Enrich.WithThreadId()
                     .Enrich.WithSpan()
                     .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                                                  .WithDefaultDestructurers()
                                                  .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                     .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                     .WriteTo.Console()
                     .WriteTo.OpenTelemetry(options =>
                     {
                         options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                     });

        builder.Logging.AddSerilog(config.CreateLogger());

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetryLogging(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
               .WithMetrics(metrics =>
               {
                   metrics.AddAspNetCoreInstrumentation()
                          .AddHttpClientInstrumentation()
                          .AddRuntimeInstrumentation();
               })
               .WithTracing(tracing =>
               {
                   tracing.AddAspNetCoreInstrumentation()

                          // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                          //.AddGrpcClientInstrumentation()
                          .AddHttpClientInstrumentation();
               });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()

               // Add a default liveness check to ensure app is responsive
               .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
        // app.MapPrometheusScrapingEndpoint();

        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
        }

        return app;
    }
}