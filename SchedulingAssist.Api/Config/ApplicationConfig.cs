using SchedulingAssist.Application.Common.Interfaces;
using SchedulingAssist.Application.Events;
using SchedulingAssist.Infrastructure.Persistence.Repositories;

namespace SchedulingAssist.Config;

public static class ApplicationConfig
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IEventService, EventService>();
        
        services.AddScoped<IEventRepository, EventRepository>();
    }
}