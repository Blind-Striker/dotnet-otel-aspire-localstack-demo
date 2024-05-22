namespace OpenTelemetry.Demo.Infrastructure.Domain.Event.Models;

public record RegisterToEventRequest(int EventId, int UserId);

public record EventModel(int Id, string Name, DateTime Date);

public record UserEventModel(int UserId, string FirstName, string LastnName, int EventId, string EventName, DateTime EventDate);