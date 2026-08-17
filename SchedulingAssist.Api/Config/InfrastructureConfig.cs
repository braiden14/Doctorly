using Microsoft.EntityFrameworkCore;
using SchedulingAssist.Infrastructure.Persistence;

namespace SchedulingAssist.Config;

public static class InfrastructureConfig
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SchedulingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DoctorScheduleConnection")));

        return services;
    }
}