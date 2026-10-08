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
            .Include(eventItem => eventItem.Department)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Event>> GetByOrganizerIdAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(eventItem => eventItem.Department)
            .Where(x => x.OrganizerId == organizerId)
            .ToListAsync(cancellationToken);
    }

    public Task<Event?> GetWithDepartmentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(eventItem => eventItem.Department)
            .FirstOrDefaultAsync(eventItem => eventItem.Id == id, cancellationToken);
    }
}