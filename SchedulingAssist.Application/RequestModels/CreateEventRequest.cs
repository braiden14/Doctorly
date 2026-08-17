namespace SchedulingAssist.Application.RequestModels;

public record CreateEventRequest(string Title, string Description, DateTimeOffset StartTime, DateTimeOffset EndTime);