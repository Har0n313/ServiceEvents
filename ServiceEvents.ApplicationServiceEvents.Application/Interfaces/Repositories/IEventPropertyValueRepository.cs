using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IEventPropertyValueRepository
    : IRepository<EventPropertyValue>
{
    Task<EventPropertyValue?> GetAsync(
        Guid eventId,
        Guid propertyId,
        CancellationToken cancellationToken = default);
}