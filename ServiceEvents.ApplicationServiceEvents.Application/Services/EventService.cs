using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Mappings;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<EventResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetAllAsync(
            cancellationToken);

        return events
            .Select(eventEntity => eventEntity.ToResponse())
            .ToList();
    }

    public async Task<EventResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            id,
            cancellationToken);

        return eventEntity?.ToResponse();
    }

    public async Task<EventResponse> CreateAsync(
        CreateEventRequest request,
        Guid organizerId,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = new Event(
            request.Title,
            request.Description,
            request.StartDate,
            organizerId,
            request.Location,
            request.EndDate,
            request.MaxParticipants,
            request.ImagePath);

        await _eventRepository.AddAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return eventEntity.ToResponse();
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await GetEventAsync(
            id,
            cancellationToken);

        eventEntity.UpdateInformation(
            request.Title,
            request.Description,
            request.StartDate,
            request.Location,
            request.EndDate,
            request.MaxParticipants,
            request.ImagePath);

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task PublishAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await GetEventAsync(
            id,
            cancellationToken);

        eventEntity.Publish();

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task CloseRegistrationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await GetEventAsync(
            id,
            cancellationToken);

        eventEntity.CloseRegistration();

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await GetEventAsync(
            id,
            cancellationToken);

        eventEntity.Cancel();

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await GetEventAsync(
            id,
            cancellationToken);

        await _eventRepository.DeleteAsync(
            eventEntity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Event> GetEventAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with id '{id}' was not found.");
        }

        return eventEntity;
    }
}