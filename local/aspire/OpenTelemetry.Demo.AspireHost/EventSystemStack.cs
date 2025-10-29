#pragma warning disable IDE0130

// ReSharper disable CheckNamespace
namespace AWSCDK.AppHost;

internal sealed class EventSystemStack : Stack
{
    public IQueue TicketQueue { get; }

    public ITopic TicketTopic { get; }

    public EventSystemStack(Construct scope, string id) : base(scope, id)
    {
        TicketQueue = new Queue(this, "TicketQueue", new QueueProps { RemovalPolicy = RemovalPolicy.DESTROY, });

        TicketTopic = new Topic(this, "TicketTopic", new TopicProps
        {
            // No additional properties in the original template.
        });

        TicketTopic.AddSubscription(new SqsSubscription(TicketQueue));

        var queuePolicy = new QueuePolicy(this, "TicketQueuePolicy", new QueuePolicyProps { Queues = [TicketQueue], });

        queuePolicy.Document.AddStatements(new PolicyStatement(new PolicyStatementProps
        {
            Effect = Effect.ALLOW,
            Principals = [new ServicePrincipal("sns.amazonaws.com")],
            Actions = ["sqs:SendMessage"],
            Resources = [TicketQueue.QueueArn],
            Conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { "ArnEquals", new Dictionary<string, object>(StringComparer.Ordinal) { { "aws:SourceArn", TicketTopic.TopicArn }, } },
            },
        }));
    }
}
