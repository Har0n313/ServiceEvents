using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventPropertyDTO
{
    public sealed record CreateEventPropertyRequest(
    string Name,
    PropertyDataType DataType);
}
