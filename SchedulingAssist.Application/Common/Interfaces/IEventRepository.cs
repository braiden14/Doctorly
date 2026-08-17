using SchedulingAssist.Domain.Events;

namespace SchedulingAssist.Application.Common.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<long> AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event @event, long userId, CancellationToken cancellationToken = default);
}