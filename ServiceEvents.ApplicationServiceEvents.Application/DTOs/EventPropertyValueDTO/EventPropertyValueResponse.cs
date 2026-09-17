namespace ServiceEvents.Application.DTOs.EventPropertyValueDTO;

public sealed record EventPropertyValueResponse(
    Guid PropertyValueId,
    Guid EventId,
    Guid PropertyId,
    string Value);