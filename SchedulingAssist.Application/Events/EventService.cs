using SchedulingAssist.Application.Common.Interfaces;

namespace SchedulingAssist.Application.Events;

public class EventService(IEventRepository eventRepository) : IEventService
{
    public async Task<long> CreateEvent(string title, string description, DateTimeOffset startTime,
        DateTimeOffset endTime, long userId, CancellationToken cancellationToken)
    {
        var @event = Domain.Events.Event.Create(
            title,
            description,
            startTime,
            endTime,
            userId);

        var eventId = await eventRepository.AddAsync(@event, cancellationToken);

        return eventId;
    }
}