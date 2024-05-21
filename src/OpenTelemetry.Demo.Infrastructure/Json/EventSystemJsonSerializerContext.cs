namespace OpenTelemetry.Demo.Infrastructure.Json;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UserModel))]
[JsonSerializable(typeof(IEnumerable<UserModel>))]
[JsonSerializable(typeof(RegisterToEventRequest))]
[JsonSerializable(typeof(EventModel))]
[JsonSerializable(typeof(EventModel[]))]
[JsonSerializable(typeof(UserEventModel))]
[JsonSerializable(typeof(IEnumerable<UserEventModel>))]
public partial class EventSystemJsonSerializerContext : JsonSerializerContext;