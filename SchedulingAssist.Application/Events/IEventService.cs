using SchedulingAssist.Application.RequestModels;

namespace SchedulingAssist.Application.Events;

public interface IEventService
{
    Task<long> CreateEvent(string title, string description, DateTimeOffset startTime, DateTimeOffset endTime,
        long userId, CancellationToken cancellationToken);

    Task UpdateAsync(long eventId, UpdateEventRequest request, long userId,
        CancellationToken cancellationToken = default);
}