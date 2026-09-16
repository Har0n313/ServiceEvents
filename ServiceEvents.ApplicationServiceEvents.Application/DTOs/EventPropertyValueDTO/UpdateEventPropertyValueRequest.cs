namespace ServiceEvents.Application.DTOs.EventPropertyValueDTO;

public sealed record UpdateEventPropertyValueRequest(
    Guid PropertyValueId,
    string Value);