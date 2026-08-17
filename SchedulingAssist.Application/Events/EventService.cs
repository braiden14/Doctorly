using Microsoft.Extensions.Logging;
using SchedulingAssist.Application.Common.Interfaces;
using SchedulingAssist.Application.RequestModels;
using SchedulingAssist.Domain.Events;

namespace SchedulingAssist.Application.Events;

public class EventService(IEventRepository eventRepository, ILogger<EventService> logger) : IEventService
{
    public async Task<long> CreateEvent(string title, string description, DateTimeOffset startTime,
        DateTimeOffset endTime, long userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating event with title {EventTitle} from {StartTime} to {EndTime}", title,
            startTime,
            endTime);

        var @event = Event.Create(title, description, startTime, endTime, userId);

        var eventId = await eventRepository.AddAsync(@event, cancellationToken);

        logger.LogInformation("Event {EventId} created successfully", eventId);

        return eventId;
    }
    
    public async Task UpdateAsync(long eventId, UpdateEventRequest request, long userId, CancellationToken cancellationToken = default)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        
        if (@event is null)
        {
            throw new KeyNotFoundException(
                $"Event with ID {eventId} was not found.");
        }
        
        logger.LogInformation("Creating event with title {EventTitle} from {StartTime} to {EndTime}", @event.Title,
            @event.StartTime, @event.EndTime);

        @event.Update(request.Title, request.Description, request.StartTime, request.EndTime);

        await eventRepository.UpdateAsync(
            @event,
            userId,
            cancellationToken);
        
        logger.LogInformation("Event {EventId} updated successfully", eventId);
    }

    public async Task CancelAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        
        if (@event is null)
        {
            throw new KeyNotFoundException($"Event with ID {eventId} was not found.");
        }
        
        logger.LogInformation("Cancelling event with title {EventTitle} from {StartTime} to {EndTime}", @event.Title,
            @event.StartTime, @event.EndTime);
        
        @event.Cancel();

        await eventRepository.UpdateAsync(@event, userId, cancellationToken);
        
        logger.LogInformation("Event {EventId} cancelled successfully", eventId);
    }

    public async Task DeleteAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        
        if (@event is null)
        {
            throw new KeyNotFoundException($"Event with ID {eventId} was not found.");
        }
        
        logger.LogInformation("Deleting event with title {EventTitle} from {StartTime} to {EndTime}", @event.Title,
            @event.StartTime, @event.EndTime);
        
        @event.Delete();

        await eventRepository.UpdateAsync(@event, userId, cancellationToken);
        
        logger.LogInformation("Event {EventId} deleted successfully", eventId);
    }
}