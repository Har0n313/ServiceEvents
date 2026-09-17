using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IEventRegistrationRepository 
    : IRepository<EventRegistration>
{
    Task<EventRegistration?> GetAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventRegistration>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventRegistration>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}