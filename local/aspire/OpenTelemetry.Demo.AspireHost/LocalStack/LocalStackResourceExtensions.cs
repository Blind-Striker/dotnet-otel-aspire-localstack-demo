namespace OpenTelemetry.Demo.AspireHost.LocalStack;

public static class LocalStackResourceExtensions
{
    private const string LocalStackSectionName = "LocalStack";

    /// <summary>
    /// Add a configuration for creating AWS SDK Clients with LocalStack configuration
    /// By default, LocalStack is disabled, unless the LocalStack section is present in the configuration and the UseLocalStack property is set to true
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ILocalStackOptions AddLocalStackConfig(this IDistributedApplicationBuilder builder)
    {
        // check if the localstack section is present in the configuration
        var localStackSection = builder.Configuration.GetSection(LocalStackSectionName);

        // if the section is not present, return default options
        if (!localStackSection.Exists())
        {
            return new LocalStackOptions();
        }

        // if the section is present, bind the section to the options
        var options = new LocalStackOptions();
        localStackSection.Bind(options, c => c.BindNonPublicProperties = true);

        return options;
    }

    /// <summary>
    /// Enable or disable the use of LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="useLocalStack"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithUseLocalStack(this ILocalStackOptions options, bool useLocalStack)
    {
        return new LocalStackOptions(useLocalStack, options.Session, options.Config);
    }

    /// <summary>
    /// Set the session options for LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="sessionOptions"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithSessionOptions(this ILocalStackOptions options, SessionOptions sessionOptions)
    {
        return new LocalStackOptions(options.UseLocalStack, sessionOptions, options.Config);
    }

    /// <summary>
    /// Set the config options for LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithConfigOptions(this ILocalStackOptions options, ConfigOptions configOptions)
    {
        return new LocalStackOptions(options.UseLocalStack, options.Session, configOptions);
    }

    /// <summary>
    /// Set the LocalStack host
    /// </summary>
    /// <param name="options"></param>
    /// <param name="localStackHost"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithLocalStackHost(this ILocalStackOptions options, string localStackHost)
    {
        var optionsConfig = options.Config;
        var configOptions = new ConfigOptions(localStackHost, optionsConfig.UseSsl, optionsConfig.UseSsl, optionsConfig.EdgePort);

        return new LocalStackOptions(options.UseLocalStack, options.Session, configOptions);
    }

    /// <summary>
    /// Set whether to use SSL for LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="useSsl"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithUseSsl(this ILocalStackOptions options, bool useSsl)
    {
        var optionsConfig = options.Config;
        var configOptions = new ConfigOptions(optionsConfig.LocalStackHost, useSsl, optionsConfig.UseSsl, optionsConfig.EdgePort);

        return new LocalStackOptions(options.UseLocalStack, options.Session, configOptions);
    }

    /// <summary>
    /// Set whether to use legacy ports for LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="useLegacyPorts"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithUseLegacyPorts(this ILocalStackOptions options, bool useLegacyPorts)
    {
        var optionsConfig = options.Config;
        var configOptions = new ConfigOptions(optionsConfig.LocalStackHost, optionsConfig.UseSsl, useLegacyPorts, optionsConfig.EdgePort);

        return new LocalStackOptions(options.UseLocalStack, options.Session, configOptions);
    }

    /// <summary>
    /// Set the edge port for LocalStack
    /// </summary>
    /// <param name="options"></param>
    /// <param name="edgePort"></param>
    /// <returns></returns>
    public static ILocalStackOptions WithEdgePort(this ILocalStackOptions options, int edgePort)
    {
        var optionsConfig = options.Config;
        var configOptions = new ConfigOptions(optionsConfig.LocalStackHost, optionsConfig.UseSsl, optionsConfig.UseSsl, edgePort);

        return new LocalStackOptions(options.UseLocalStack, options.Session, configOptions);
    }
}