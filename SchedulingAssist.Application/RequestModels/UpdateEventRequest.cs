namespace SchedulingAssist.Application.RequestModels;

public record UpdateEventRequest(
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime);