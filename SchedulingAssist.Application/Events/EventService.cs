using Microsoft.Extensions.Logging;
using SchedulingAssist.Application.Common.Interfaces;
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
}