using SchedulingAssist.Domain.Events;

namespace SchedulingAssist.Application.Common.Interfaces;

public interface IEventRepository
{
    Task<long> AddAsync(Event @event, CancellationToken cancellationToken = default);
}