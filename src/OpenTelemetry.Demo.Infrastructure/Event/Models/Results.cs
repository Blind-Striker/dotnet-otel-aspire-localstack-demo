using System.Collections.Immutable;

namespace OpenTelemetry.Demo.Infrastructure.Event.Models;

[GenerateOneOf]
public partial class RegisterToEventResult : OneOfBase<UserEventModel, NotFound, ValidationFailed>;

[GenerateOneOf]
public partial class GetEventResult : OneOfBase<EventModel, NotFound>;

[GenerateOneOf]
public partial class GetEventsResult : OneOfBase<EventModel[]>;