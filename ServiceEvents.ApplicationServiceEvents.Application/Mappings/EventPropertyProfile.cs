using ServiceEvents.Application.DTOs.EventPropertyDTO;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Mappings;

public static class EventPropertyProfile
{
    public static EventPropertyResponse ToResponse(
        this EventProperty entity)
    {
        return new EventPropertyResponse(
            entity.Id,
            entity.Name,
            entity.DataType,
            entity.IsActive);
    }
}