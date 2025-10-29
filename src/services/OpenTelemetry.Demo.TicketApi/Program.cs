var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolver = EventSystemJsonSerializerContext.Default);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
   .WithName("CreateTicket")
   .WithOpenApi();

await app.RunAsync();
