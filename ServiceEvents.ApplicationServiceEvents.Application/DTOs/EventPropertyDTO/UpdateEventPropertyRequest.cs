using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventPropertyDTO
{
    public sealed record UpdateEventPropertyRequest(
    Guid PropertyId,
    string Name,
    PropertyDataType DataType);
}
