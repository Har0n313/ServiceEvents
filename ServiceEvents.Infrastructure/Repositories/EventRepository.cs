using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public class EventRepository
    : Repository<Event>, IEventRepository
{
    public EventRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<Event>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetByOrganizerIdAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.OrganizerId == organizerId)
            .ToListAsync(cancellationToken);
    }
}