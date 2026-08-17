using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulingAssist.Infrastructure.Persistence.Models;

namespace SchedulingAssist.Infrastructure.Persistence.Configuration;

public sealed class EventConfiguration
    : IEntityTypeConfiguration<EventModel>
{
    public void Configure(
        EntityTypeBuilder<EventModel> builder)
    {
        builder.ToTable("Event");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.HasIndex(x => x.StartTime);
    }
}