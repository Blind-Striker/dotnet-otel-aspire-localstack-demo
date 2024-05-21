WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEventSystemDbContext();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolver = EventSystemJsonSerializerContext.Default);
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/user", async (IUserService userService, CreateUserRequest request) =>
   {
       CreateUserResult result = await userService.CreateUserAsync(request);

       return result.Match<IResult>(
           model => TypedResults.CreatedAtRoute(model, "GetUser", new { id = model.Id }),
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()));
   })
   .WithName("CreateUser")
   .WithOpenApi();

app.MapGet("/user/{id:int}", async (IUserService userService, int id) =>
   {
       GetUserResult result = await userService.GetUserByIdAsync(id);

       return result.Match<IResult>(
           model => TypedResults.Ok(model),
           _ => TypedResults.NotFound());
   })
   .WithName("GetUser")
   .WithOpenApi();

app.MapGet("/event", async (IEventService eventService) =>
   {
       GetEventsResult result = await eventService.GetEventsAsync();

       return result.Match<IResult>(events => TypedResults.Ok(events));
   })
   .WithName("GetEvents")
   .WithOpenApi();

app.MapGet("/event/{id:int}", async (IEventService eventService, int id) =>
   {
       GetEventResult result = await eventService.GetEventByIdAsync(id);

       return result.Match<IResult>(
           model => TypedResults.Ok(model),
           _ => TypedResults.NotFound());
   })
   .WithName("GetEvent")
   .WithOpenApi();

app.MapPost("/event/register", async (IEventService eventService, RegisterToEventRequest request) =>
   {
       RegisterToEventResult result = await eventService.RegisterToEventAsync(request);

       return result.Match<IResult>(
           model => TypedResults.CreatedAtRoute(model, "GetEvent", new { id = model.EventId }),
           _ => TypedResults.NotFound(),
           validationFailed => TypedResults.BadRequest(validationFailed.Errors.ToJson()));
   })
   .WithName("AttendEvent")
   .WithOpenApi();

await app.RunAsync();