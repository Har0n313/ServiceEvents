using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public class EventPropertyRepository
    : Repository<EventProperty>, IEventPropertyRepository
{
    public EventPropertyRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<EventProperty>> GetGlobalAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventProperty>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await Context.EventPropertyValues
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .Select(x => x.Property)
            .ToListAsync(cancellationToken);
    }
}