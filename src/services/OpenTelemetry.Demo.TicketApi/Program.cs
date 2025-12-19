using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolver = EventSystemJsonSerializerContext.Default);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/ticket", async (ITicketService ticketService, CreateTicketRequest request) =>
   {
       var result = await ticketService.CreateTicketAsync(request);

       return result.Match<IResult>(
           TypedResults.Ok,
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()),
           _ => TypedResults.NotFound());
   })
   .WithName("CreateTicket");

await app.RunAsync();
