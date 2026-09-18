using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public class EventPropertyValueRepository
    : Repository<EventPropertyValue>, IEventPropertyValueRepository
{
    public EventPropertyValueRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<EventPropertyValue?> GetAsync(
        Guid eventId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(
                x => x.EventId == eventId &&
                     x.PropertyId == propertyId,
                cancellationToken);
    }
}