using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public class EventRegistrationRepository
    : Repository<EventRegistration>, IEventRegistrationRepository
{
    public EventRegistrationRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<EventRegistration?> GetAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(
                x => x.EventId == eventId &&
                     x.UserId == userId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventRegistration>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventRegistration>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}