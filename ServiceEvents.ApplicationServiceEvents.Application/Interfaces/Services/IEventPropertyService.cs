using ServiceEvents.Application.DTOs.EventPropertyDTO;

namespace ServiceEvents.Application.Interfaces.Services;

public interface IEventPropertyService
{
    Task<IReadOnlyCollection<EventPropertyResponse>> GetGlobalPropertiesAsync(
        CancellationToken cancellationToken = default);

    Task<EventPropertyResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EventPropertyResponse> CreateAsync(
        CreateEventPropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<EventPropertyResponse> CreateForEventAsync(
        Guid eventId,
        CreateEventPropertyRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateEventPropertyRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddToEventAsync(
        Guid eventId,
        Guid propertyId,
        string value,
        CancellationToken cancellationToken = default);

    Task RemoveFromEventAsync(
        Guid eventId,
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task UpdateEventPropertyValueAsync(
        Guid eventId,
        Guid propertyId,
        string value,
        CancellationToken cancellationToken = default);
}