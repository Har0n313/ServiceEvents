using ServiceEvents.Application.DTOs.EventRegistrationDTO;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Mappings;

public static class EventRegistrationProfile
{
    public static EventRegistrationResponse ToResponse(
        this EventRegistration entity)
    {
        return new EventRegistrationResponse(
            entity.Id,
            entity.EventId,
            entity.UserId,
            entity.RegisteredAt,
            entity.Status);
    }
}