using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Infrastructure.EntityFramework.Configuration;

public class EventPropertyConfiguration
    : IEntityTypeConfiguration<EventProperty>
{
    public void Configure(EntityTypeBuilder<EventProperty> builder)
    {
        builder.ToTable("EventProperties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DataType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Values)
            .WithOne(x => x.Property)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}