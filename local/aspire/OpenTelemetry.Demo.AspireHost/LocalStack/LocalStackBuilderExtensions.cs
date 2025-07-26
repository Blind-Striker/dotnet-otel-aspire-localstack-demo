namespace OpenTelemetry.Demo.AspireHost.LocalStack;

public static class LocalStackBuilderExtensions
{
    public static IResourceBuilder<LocalStackResource> AddLocalStack(this IDistributedApplicationBuilder builder, string name, ILocalStackOptions? options = null)
    {
        var localStackOptions = options ?? new LocalStackOptions();

        var localstack = new LocalStackResource(name, localStackOptions);

        return builder.AddResource(localstack)
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithImage(LocalStackContainerImageTags.Image, LocalStackContainerImageTags.Tag)
                      .WithImageRegistry(LocalStackContainerImageTags.Registry)
                      .WithEndpoint(port: localStackOptions.Config.EdgePort, targetPort: 4566, scheme: "http", name: "http", isExternal: true)
                      .WithHttpHealthCheck("/_localstack/health", 200, "http")
                      .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
                      .WithEnvironment("DEBUG", "0")
                      .WithExternalHttpEndpoints();
    }
}