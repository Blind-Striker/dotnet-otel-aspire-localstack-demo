using static System.Environment;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolver = EventSystemJsonSerializerContext.Default);
services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IEventService, EventService>();

builder.Services
       .AddLocalStack(builder.Configuration)
       .AddAwsService<IAmazonSimpleNotificationService>();

var awsResources = new AwsResources();
builder.Configuration.Bind("AWS:Resources", awsResources);

var ticketIntegration = builder.Configuration.GetValue<string>("EventSystem:TicketIntegration");

// Configuring messaging using the AWS.Messaging library.
builder.Services.AddAWSMessageBus(messageBuilder =>
{
    messageBuilder.AddMessageSource("event-api");

    messageBuilder.AddSNSPublisher<CreateTicketRequest>(awsResources.TicketTopicArn);
});

// Register factory for ITicketBookingClient
if (string.Equals(ticketIntegration, "aws", StringComparison.Ordinal))
{
    services.AddScoped<ITicketBookingClient, TicketAwsMessagingClient>();
}
else
{
    services.AddHttpClient<ITicketBookingClient, TicketBookingHttpClient>(client =>
    {
        var baseAddress = configuration.GetValue<string>("TicketBookingClient:BaseAddress") ??
                          GetEnvironmentVariable("services__ticket-api__https__0") ??
                          GetEnvironmentVariable("services__ticket-api__http__0") ??
                          throw new InvalidOperationException("TicketBookingClient:BaseAddress is not configured.");

        client.BaseAddress = new Uri(baseAddress);
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/user", async (IUserService userService, CreateUserRequest request) =>
   {
       var result = await userService.CreateUserAsync(request);

       return result.Match<IResult>(
           model => TypedResults.CreatedAtRoute(model, "GetUser", new { id = model.Id }),
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()));
   })
   .WithName("CreateUser")
   .WithOpenApi();

app.MapGet("/user/{id:int}", async (IUserService userService, int id) =>
   {
       var result = await userService.GetUserByIdAsync(id);

       return result.Match<IResult>(
           TypedResults.Ok,
           _ => TypedResults.NotFound());
   })
   .WithName("GetUser")
   .WithOpenApi();

app.MapGet("/event", async (IEventService eventService) =>
   {
       var result = await eventService.GetEventsAsync();

       return result.Match<IResult>(TypedResults.Ok);
   })
   .WithName("GetEvents")
   .WithOpenApi();

app.MapGet("/event/{id:int}", async (IEventService eventService, int id) =>
   {
       var result = await eventService.GetEventByIdAsync(id);

       return result.Match<IResult>(
           TypedResults.Ok,
           _ => TypedResults.NotFound());
   })
   .WithName("GetEvent")
   .WithOpenApi();

app.MapPost("/event/register", async (IEventService eventService, RegisterToEventRequest request) =>
   {
       // using var activity = activitySource.StartActivity("POST.EventApi.AttendEvent", ActivityKind.Client);

       var result = await eventService.RegisterToEventAsync(request);

       return result.Match<IResult>(
           model => TypedResults.CreatedAtRoute(model, "GetEvent", new { id = model.EventId }),
           _ => TypedResults.NotFound(),
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()));
   })
   .WithName("AttendEvent")
   .WithOpenApi();

await app.RunAsync();
