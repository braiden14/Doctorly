using Microsoft.EntityFrameworkCore;
using SchedulingAssist.Infrastructure.Persistence.Models;

namespace SchedulingAssist.Infrastructure.Persistence;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<EventModel> Events => Set<EventModel>();
    public DbSet<EventHistoryModel> EventHistory => Set<EventHistoryModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}