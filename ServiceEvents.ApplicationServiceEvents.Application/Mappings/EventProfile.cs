using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Mappings;

public static class EventProfile
{
    public static EventResponse ToResponse(this Event entity)
    {
        return new EventResponse(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.StartDate,
            entity.EndDate,
            entity.Location,
            entity.MaxParticipants,
            entity.ImagePath,
            entity.Status,
            entity.OrganizerId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}