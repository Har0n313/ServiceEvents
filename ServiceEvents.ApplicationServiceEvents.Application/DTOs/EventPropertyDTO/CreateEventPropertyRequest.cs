using System.ComponentModel.DataAnnotations;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventPropertyDTO
{
    public sealed record CreateEventPropertyRequest(
    [param: Required]
    [param: StringLength(200)]
    string Name,
    [param: EnumDataType(typeof(PropertyDataType))]
    PropertyDataType DataType);
}
