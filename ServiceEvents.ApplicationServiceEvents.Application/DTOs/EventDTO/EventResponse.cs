using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventDTO
{
    public sealed record EventResponse(
    Guid EventId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? Location,
    int? MaxParticipants,
    string? ImagePath,
    EventStatus Status,
    Guid OrganizerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);   
}
