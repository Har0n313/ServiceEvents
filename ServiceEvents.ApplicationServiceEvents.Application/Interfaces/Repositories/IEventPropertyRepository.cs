using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IEventPropertyRepository 
    : IRepository<EventProperty>
{
    Task<IReadOnlyCollection<EventProperty>> GetGlobalAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventProperty>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);
}