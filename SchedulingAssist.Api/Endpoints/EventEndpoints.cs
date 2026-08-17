using SchedulingAssist.Application.Events;
using SchedulingAssist.Application.RequestModels;

namespace SchedulingAssist.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/events", CreateEvent).WithName("CreateEvent")
            .WithSummary("Create an event")
            .WithDescription("Creates a new event in the doctor's schedule.")
            .Produces<long>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        endpoints.MapPut("/{id:long}", UpdateEvent);
        ;
    }

    private static async Task<IResult> CreateEvent(CreateEventRequest request, IEventService eventService,
        CancellationToken cancellationToken)
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
    
    private static async Task<IResult> UpdateEvent(
        long id,
        UpdateEventRequest request,
        IEventService eventService,
        CancellationToken cancellationToken)
    {
        long userId = 1;
        
        await eventService.UpdateAsync(
            id,
            request,
            userId,
            cancellationToken);

        return Results.NoContent();
    }
}

