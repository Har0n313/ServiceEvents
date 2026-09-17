using ServiceEvents.Application.DTOs.EventPropertyDTO;

namespace ServiceEvents.Application.Interfaces.Services;

public interface IEventPropertyService
{
    Task<IReadOnlyCollection<EventPropertyResponse>> GetGlobalAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EventPropertyResponse>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<EventPropertyResponse> CreateAsync(
        CreateEventPropertyRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateEventPropertyRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task SetValueAsync(
        Guid eventId,
        Guid propertyId,
        string value,
        CancellationToken cancellationToken = default);

    Task RemoveValueAsync(
        Guid eventId,
        Guid propertyId,
        CancellationToken cancellationToken = default);
}