using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Infrastructure.EntityFramework.Configuration;

public class EventPropertyValueConfiguration
    : IEntityTypeConfiguration<EventPropertyValue>
{
    public void Configure(EntityTypeBuilder<EventPropertyValue> builder)
    {
        builder.ToTable("EventPropertyValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.PropertyValues)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Property)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
            {
                x.EventId,
                x.PropertyId
            })
            .IsUnique();
    }
}