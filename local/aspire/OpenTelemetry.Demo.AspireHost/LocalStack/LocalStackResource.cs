namespace OpenTelemetry.Demo.AspireHost.LocalStack;

public sealed class LocalStackResource(string name, ILocalStackOptions options) : ContainerResource(name), IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "http";

    private EndpointReference? _primaryEndpoint;

    public EndpointReference PrimaryEndpoint => _primaryEndpoint ??= new(this, PrimaryEndpointName);

    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{PrimaryEndpoint.Property(EndpointProperty.Url)}");
}