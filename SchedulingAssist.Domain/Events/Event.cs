namespace SchedulingAssist.Domain.Events;

public class Event
{
    public long Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public long CreatedByUserId { get; set; }
    public bool IsDeleted { get; private set; }
    public bool IsCancelled { get; private set; }

    public Event()
    {
    }

    public static Event Rehydrate(long id, string title, string description, DateTimeOffset startTime,
        DateTimeOffset endTime, long createdByUserId, bool isCancelled = false, bool isDeleted = false)
    {
        return new Event
        {
            Id = id,
            Title = title,
            Description = description,
            StartTime = startTime,
            EndTime = endTime,
            CreatedByUserId = createdByUserId,
            IsCancelled = isCancelled,
            IsDeleted = isDeleted
        };
    }

    private Event(string title, string description, DateTimeOffset startTime, DateTimeOffset endTime, long userId)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        CreatedByUserId = userId;
    }

    public static Event Create(string title, string description, DateTimeOffset startTime, DateTimeOffset endTime,
        long userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title is required.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Event title cannot exceed 200 characters.", nameof(title));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Event description cannot exceed 2000 characters.", nameof(description));

        if (startTime >= endTime)
            throw new ArgumentException("Event start time must be before the end time.");

        return new Event(title.Trim(), description.Trim(), startTime, endTime, userId);
    }

    public void Update(string title, string description, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));

        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void Cancel()
    {
        if (IsDeleted)
            throw new InvalidOperationException("A deleted event cannot be cancelled.");

        if (IsCancelled)
            throw new InvalidOperationException("Event is already cancelled.");

        IsCancelled = true;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Event is already deleted.");

        IsDeleted = true;
    }
}