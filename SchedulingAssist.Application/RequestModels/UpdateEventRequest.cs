namespace SchedulingAssist.Application.RequestModels;

public record UpdateEventRequest(
    string Title,
    string Description,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);