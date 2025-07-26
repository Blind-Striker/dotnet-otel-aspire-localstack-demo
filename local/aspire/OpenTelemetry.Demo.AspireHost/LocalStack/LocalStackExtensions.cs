namespace OpenTelemetry.Demo.AspireHost.LocalStack;

public static class LocalStackExtensions
{
    public static IResourceBuilder<ICloudFormationTemplateResource> WithLocalStack(this IResourceBuilder<ICloudFormationTemplateResource> builder, ILocalStackOptions? options = null)
    {
        var localStackOptions = options ?? new LocalStackOptions();

        if (!localStackOptions.UseLocalStack)
        {
            return builder;
        }

        var amazonCloudFormationClient = SessionStandalone.Init()
                                                          .WithSessionOptions(localStackOptions.Session)
                                                          .WithConfigurationOptions(localStackOptions.Config)
                                                          .Create()
                                                          .CreateClientByImplementation<AmazonCloudFormationClient>();

        builder.Resource.CloudFormationClient = amazonCloudFormationClient;

        return builder;
    }
}