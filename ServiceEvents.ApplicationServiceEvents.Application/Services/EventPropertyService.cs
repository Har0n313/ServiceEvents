

using ServiceEvents.Application.DTOs.EventPropertyDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Mappings;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Services;

public class EventPropertyService : IEventPropertyService
{
    private readonly IEventPropertyRepository _propertyRepository;
    private readonly IEventPropertyValueRepository _propertyValueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventPropertyService(
        IEventPropertyRepository propertyRepository,
        IEventPropertyValueRepository propertyValueRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _propertyValueRepository = propertyValueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<EventPropertyResponse>> GetGlobalAsync(
        CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetGlobalAsync(
            cancellationToken);

        return properties
            .Select(property => property.ToResponse())
            .ToList();
    }

    public async Task<IReadOnlyCollection<EventPropertyResponse>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetByEventIdAsync(
            eventId,
            cancellationToken);

        return properties
            .Select(property => property.ToResponse())
            .ToList();
    }

    public async Task<EventPropertyResponse> CreateAsync(
        CreateEventPropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = new EventProperty(
            request.Name,
            request.DataType);

        await _propertyRepository.AddAsync(
            property,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return property.ToResponse();
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateEventPropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await GetPropertyAsync(
            id,
            cancellationToken);

        property.Update(
            request.Name,
            request.DataType);

        await _propertyRepository.UpdateAsync(
            property,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var property = await GetPropertyAsync(
            id,
            cancellationToken);

        await _propertyRepository.DeleteAsync(
            property,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SetValueAsync(
        Guid eventId,
        Guid propertyId,
        string value,
        CancellationToken cancellationToken = default)
    {
        var property = await GetPropertyAsync(
            propertyId,
            cancellationToken);

        var propertyValue = await _propertyValueRepository.GetAsync(
            eventId,
            propertyId,
            cancellationToken);

        if (propertyValue is null)
        {
            propertyValue = new EventPropertyValue(
                eventId,
                propertyId,
                value);

            await _propertyValueRepository.AddAsync(
                propertyValue,
                cancellationToken);
        }
        else
        {
            propertyValue.UpdateValue(value);

            await _propertyValueRepository.UpdateAsync(
                propertyValue,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveValueAsync(
        Guid eventId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var propertyValue = await _propertyValueRepository.GetAsync(
            eventId,
            propertyId,
            cancellationToken);

        if (propertyValue is null)
            return;

        await _propertyValueRepository.DeleteAsync(
            propertyValue,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<EventProperty> GetPropertyAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (property is null)
            throw new KeyNotFoundException(
                $"Event property with id '{id}' was not found.");

        return property;
    }
}