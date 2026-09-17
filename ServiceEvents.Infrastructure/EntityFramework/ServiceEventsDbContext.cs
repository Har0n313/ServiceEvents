using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Infrastructure.EntityFramework;

public class ServiceEventsDbContext : DbContext
{
    public ServiceEventsDbContext(
        DbContextOptions<ServiceEventsDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<EventProperty> EventProperties => Set<EventProperty>();
    public DbSet<EventPropertyValue> EventPropertyValues => Set<EventPropertyValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }
}