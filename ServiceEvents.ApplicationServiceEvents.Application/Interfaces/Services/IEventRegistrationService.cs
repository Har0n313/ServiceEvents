using ServiceEvents.Application.DTOs.EventRegistrationDTO;

namespace ServiceEvents.Application.Interfaces.Services;

public interface IEventRegistrationService
{
    Task<EventRegistrationResponse> RegisterAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventRegistrationResponse>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventRegistrationResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsRegisteredAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default);
}