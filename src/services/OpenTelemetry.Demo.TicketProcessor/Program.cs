var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

var awsResources = new AWSResources();
builder.Configuration.Bind("AWS:Resources", awsResources);

if (string.IsNullOrEmpty(awsResources.TicketQueueUrl))
{
    throw new InvalidOperationException("Missing required configuration for feedback queue url");
}

builder.Services.AddLocalStack(builder.Configuration)
       .AddAWSServiceLocalStack<IAmazonSQS>();

builder.Services.AddAWSMessageBus(busBuilder =>
{
    busBuilder.AddSQSPoller(awsResources.TicketQueueUrl);

    busBuilder.AddMessageHandler<TicketMessageHandler, CreateTicketRequest>();
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

await app.RunAsync();
