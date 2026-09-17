namespace ServiceEvents.Application.DTOs.EventPropertyValueDTO;

public sealed record CreateEventPropertyValueRequest(
    Guid EventId,
    Guid PropertyId,
    string Value);