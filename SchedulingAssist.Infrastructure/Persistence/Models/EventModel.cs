using SchedulingAssist.Domain.Events;

namespace SchedulingAssist.Infrastructure.Persistence.Models;

public class EventModel
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long CreatedByUserId { get; set; }

}