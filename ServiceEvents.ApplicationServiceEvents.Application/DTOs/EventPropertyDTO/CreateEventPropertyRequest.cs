using System.ComponentModel.DataAnnotations;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventPropertyDTO
{
    public sealed record CreateEventPropertyRequest(
    [property: Required]
    [property: StringLength(200)]
    string Name,
    [property: EnumDataType(typeof(PropertyDataType))]
    PropertyDataType DataType);
}
