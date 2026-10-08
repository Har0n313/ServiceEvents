using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Infrastructure.EntityFramework.Configuration;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(department => department.Id);
        builder.Property(department => department.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(department => department.Name)
            .IsUnique();
    }
}
