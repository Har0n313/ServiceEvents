namespace ServiceEvents.Application.DTOs.EventDTO
{
    public sealed record UpdateEventRequest(
    Guid EventId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? Location,
    int? MaxParticipants,
    string? ImagePath);
}
