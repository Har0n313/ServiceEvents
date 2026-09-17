using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<IReadOnlyCollection<Event>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Event>> GetByOrganizerIdAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default);
}