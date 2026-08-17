using SchedulingAssist.Domain.Events;
using SchedulingAssist.Infrastructure.Persistence.Models;

namespace SchedulingAssist.Infrastructure.Persistence.Mappings;

public static class EventMappings
{
    public static Event ToDomain(this EventModel model)
    {
        return Event.Rehydrate(
            model.Id,
            model.Title,
            model.Description,
            model.StartTime,
            model.EndTime,
            model.CreatedByUserId,
            model.IsCancelled,
            model.IsDeleted);
    }

    public static EventModel ToModel(this Event @event)
    {
        return new EventModel
        {
            Id = @event.Id,
            Title = @event.Title,
            Description = @event.Description,
            StartTime = @event.StartTime,
            EndTime = @event.EndTime,
            CreatedByUserId = @event.CreatedByUserId,
            IsCancelled = @event.IsCancelled,
            IsDeleted = @event.IsDeleted,
        };
    }
}