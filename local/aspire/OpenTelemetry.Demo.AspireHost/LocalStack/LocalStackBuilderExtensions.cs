namespace OpenTelemetry.Demo.AspireHost.LocalStack;

public static class LocalStackBuilderExtensions
{
    public static IResourceBuilder<LocalStackResource> AddLocalStack(this IDistributedApplicationBuilder builder, string name, ILocalStackOptions? options = null)
    {
        ILocalStackOptions localStackOptions = options ?? new LocalStackOptions();

        var localstack = new LocalStackResource(name, localStackOptions);

        return builder.AddResource(localstack)

                      // .WithEndpoint(port: localStackOptions.Config.EdgePort, targetPort: 4566, scheme: scheme, name: localStackOptions.Config.LocalStackHost)
                      .WithHttpEndpoint(port: localStackOptions.Config.EdgePort, targetPort: 4566, name: LocalStackResource.PrimaryEndpointName)
                      .WithImage(LocalStackContainerImageTags.Image, LocalStackContainerImageTags.Tag)
                      .WithImageRegistry(LocalStackContainerImageTags.Registry)
                      .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                      .WithEnvironment("DEBUG", "1");
    }
}