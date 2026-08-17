using SchedulingAssist.Application.Events;

namespace SchedulingAssist.Endpoints;

public static class EventEndpoints
{
    public static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/events", CreateEvent);

        return endpoints;
    }

    private static async Task<IResult> CreateEvent(CreateEventRequest request, IEventService eventService, CancellationToken cancellationToken)
    {
        long userId = 1;
        
        var eventId = await eventService.CreateEvent(
            request.Title,
            request.Description,
            request.StartTime,
            request.EndTime,
            userId,
            cancellationToken);

        return Results.Created($"/api/events/{eventId}", new { id = eventId });
    }
}

public record CreateEventRequest(string Title,
    string Description,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);