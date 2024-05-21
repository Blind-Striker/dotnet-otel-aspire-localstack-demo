namespace OpenTelemetry.Demo.Infrastructure.Event.Interfaces;

public interface IEventService
{
    Task<GetEventResult> GetEventByIdAsync(int id);

    Task<GetEventsResult> GetEventsAsync();

    Task<RegisterToEventResult> RegisterToEventAsync(RegisterToEventRequest request);
}