using OpenTelemetry.Demo.Infrastructure.Common;

const string activitySourceName = "OpenTelemetry.Demo.EventApi";
ActivitySource activitySource = new(activitySourceName);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolver = EventSystemJsonSerializerContext.Default);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(activitySourceName))
       .WithTracing(tracing => tracing.AddSource("OpenTelemetry.Demo.Infrastructure"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/ticket", async (ITicketService ticketService, CreateTicketRequest request) =>
   {
       CreateTicketResult result = await ticketService.CreateTicketAsync(request);

       return result.Match<IResult>(
           model => TypedResults.Ok(model),
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()),
           _ => TypedResults.NotFound());
   })
   .WithName("CreateTicket")
   .WithOpenApi();

await app.RunAsync();