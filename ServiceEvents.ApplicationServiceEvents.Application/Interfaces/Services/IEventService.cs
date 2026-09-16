using ServiceEvents.Application.DTOs.EventDTO;

namespace ServiceEvents.Application.Interfaces.Services;

internal interface IEventService
{
    Task<IReadOnlyCollection<EventResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<EventResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EventResponse> CreateAsync(
        CreateEventRequest request,
        Guid organizerId,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken = default);

    Task PublishAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CloseRegistrationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}