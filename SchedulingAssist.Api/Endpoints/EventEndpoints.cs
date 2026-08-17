using SchedulingAssist.Application.Events;
using SchedulingAssist.Application.RequestModels;

namespace SchedulingAssist.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var events = endpoints.MapGroup("/api/events");

        events.MapPost("/", CreateEvent)
            .WithName("CreateEvent")
            .WithSummary("Create an event")
            .WithDescription("Creates a new event in the doctor's schedule.")
            .Produces<long>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        events.MapPut("/{id:long}", UpdateEvent)
            .WithName("UpdateEvent")
            .WithSummary("Update an event")
            .WithDescription("Updates an existing event in the doctor's schedule.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        events.MapPatch("/{id:long}/cancel", CancelEvent)
            .WithName("CancelEvent")
            .WithSummary("Cancel an event")
            .WithDescription("Cancels an existing event in the doctor's schedule.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        events.MapDelete("/{id:long}", DeleteEvent)
            .WithName("DeleteEvent")
            .WithSummary("Delete an event")
            .WithDescription("Soft deletes an existing event from the doctor's schedule.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
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

    private static async Task<IResult> UpdateEvent(long id, UpdateEventRequest request, IEventService eventService,
        CancellationToken cancellationToken)
    {
        long userId = 1;

        await eventService.UpdateAsync(id, request, userId, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> CancelEvent(long id, IEventService eventService,
        CancellationToken cancellationToken)
    {
        long userId = 1;

        await eventService.CancelAsync(id, userId, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteEvent(long id, IEventService eventService,
        CancellationToken cancellationToken)
    {
        long userId = 1;

        await eventService.DeleteAsync(id, userId, cancellationToken);

        return Results.NoContent();
    }
}