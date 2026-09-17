using ServiceEvents.Application.DTOs.EventRegistrationDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Mappings;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.Services;

public class EventRegistrationService : IEventRegistrationService
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventRegistrationService(
        IEventRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventRegistrationResponse> RegisterAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            eventId,
            cancellationToken);

        if (eventEntity is null)
        {
            throw new KeyNotFoundException(
                $"Event with id '{eventId}' was not found.");
        }

        if (eventEntity.Status != EventStatus.Published)
        {
            throw new InvalidOperationException(
                "Registration is not available for this event.");
        }

        var existingRegistration =
            await _registrationRepository.GetAsync(
                eventId,
                userId,
                cancellationToken);

        if (existingRegistration is not null)
        {
            if (existingRegistration.Status == RegistrationStatus.Active)
            {
                throw new InvalidOperationException(
                    "User is already registered for this event.");
            }

            existingRegistration.Restore();

            await _registrationRepository.UpdateAsync(
                existingRegistration,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return existingRegistration.ToResponse();
        }

        if (eventEntity.MaxParticipants.HasValue)
        {
            var registrations =
                await _registrationRepository.GetByEventIdAsync(
                    eventId,
                    cancellationToken);

            var activeRegistrations = registrations.Count(
                x => x.Status == RegistrationStatus.Active);

            if (activeRegistrations >= eventEntity.MaxParticipants.Value)
            {
                throw new InvalidOperationException(
                    "The maximum number of participants has been reached.");
            }
        }

        var registration = new EventRegistration(
            eventId,
            userId);

        await _registrationRepository.AddAsync(
            registration,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return registration.ToResponse();
    }

    public async Task CancelAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetAsync(
            eventId,
            userId,
            cancellationToken);

        if (registration is null)
        {
            throw new KeyNotFoundException(
                "Registration was not found.");
        }

        if (registration.Status == RegistrationStatus.Cancelled)
        {
            return;
        }

        registration.Cancel();

        await _registrationRepository.UpdateAsync(
            registration,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<EventRegistrationResponse>>
        GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
    {
        var registrations =
            await _registrationRepository.GetByEventIdAsync(
                eventId,
                cancellationToken);

        return registrations
            .Select(x => x.ToResponse())
            .ToList();
    }

    public async Task<IReadOnlyCollection<EventRegistrationResponse>>
        GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var registrations =
            await _registrationRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        return registrations
            .Select(x => x.ToResponse())
            .ToList();
    }

    public async Task<bool> IsRegisteredAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetAsync(
            eventId,
            userId,
            cancellationToken);

        return registration?.Status == RegistrationStatus.Active;
    }
}