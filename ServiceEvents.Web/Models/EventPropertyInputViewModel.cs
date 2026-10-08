using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Models;

public sealed record EventPropertyInputViewModel(
    Guid PropertyId,
    string Name,
    PropertyDataType DataType);
