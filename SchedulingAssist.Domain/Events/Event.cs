namespace SchedulingAssist.Domain.Events;

public class Event
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }
    public long CreatedByUserId { get; set; }

    private Event(
        string title,
        string description,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        long userId)
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        CreatedByUserId = userId;
    }

    public static Event Create(
        string title,
        string description,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        long userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Event title is required.",
                nameof(title));

        if (title.Length > 200)
            throw new ArgumentException(
                "Event title cannot exceed 200 characters.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException(
                "Event description cannot exceed 2000 characters.",
                nameof(description));

        if (startTime >= endTime)
            throw new ArgumentException(
                "Event start time must be before the end time.");

        return new Event(
            title.Trim(),
            description.Trim(),
            startTime,
            endTime,
            userId);
    }
}