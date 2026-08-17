using Microsoft.EntityFrameworkCore;
using SchedulingAssist.Application.Common.Interfaces;
using SchedulingAssist.Domain.Events;
using SchedulingAssist.Infrastructure.Persistence.Mappings;
using SchedulingAssist.Infrastructure.Persistence.Models;

namespace SchedulingAssist.Infrastructure.Persistence.Repositories;

public class EventRepository(SchedulingDbContext context) : IEventRepository
{
    public async Task<Event?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var eventModel = await context.Events.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return eventModel.ToDomain();
    }

    public async Task<long> AddAsync(Event @event, CancellationToken cancellationToken  = default)
    {
        var model = new EventModel
        {
            Title = @event.Title,
            Description = @event.Description,
            StartTime = @event.StartTime,
            EndTime = @event.EndTime,
            CreatedByUserId =  @event.CreatedByUserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        await context.Events.AddAsync(model, cancellationToken);
        
        await AddEventHistory(model, @event.CreatedByUserId, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return model.Id;
    }

    public async Task UpdateAsync(Event @event, long userId, CancellationToken cancellationToken = default)
    {
        var eventModel = @event.ToModel();
        
        context.Events.Update(eventModel);
        
        await AddEventHistory(eventModel, userId, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddEventHistory(EventModel model, long userId, CancellationToken cancellationToken)
    {
        var history = new EventHistoryModel
        {
            EventId = model.Id,
            Title = model.Title,
            Description = model.Description,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            CreatedAt = model.CreatedAt,
            UpdatedAt = DateTime.Now,
            CreateByUserId = userId
        };
        
        await context.EventHistory.AddAsync(history, cancellationToken);
    }
}