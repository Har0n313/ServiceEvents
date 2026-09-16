using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventPropertyDTO
{
    public sealed record EventPropertyResponse(
    Guid PropertyId,
    string Name,
    PropertyDataType DataType,
    bool IsActive);
}
