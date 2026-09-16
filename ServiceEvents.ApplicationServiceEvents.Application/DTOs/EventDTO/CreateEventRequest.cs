namespace ServiceEvents.Application.DTOs.EventDTO;

public sealed record CreateEventRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? Location,
    int? MaxParticipants,
    string? ImagePath);