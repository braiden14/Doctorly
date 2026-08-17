using SchedulingAssist.Application.Common.Interfaces;
using SchedulingAssist.Domain.Events;
using SchedulingAssist.Infrastructure.Persistence.Models;

namespace SchedulingAssist.Infrastructure.Persistence.Repositories;

public class EventRepository(SchedulingDbContext context) : IEventRepository
{
    public async Task<long> AddAsync(Event @event, CancellationToken cancellationToken)
    {
        var model = new EventModel
        {
            Title = @event.Title,
            Description = @event.Description,
            StartTime = @event.StartTime,
            EndTime = @event.EndTime
        };

        await context.Events.AddAsync(model, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return model.Id;
    }
}