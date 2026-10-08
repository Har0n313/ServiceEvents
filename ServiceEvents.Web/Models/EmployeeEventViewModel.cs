using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Domain.Enums;
using ServiceEvents.Application.DTOs.EventPropertyDTO;

namespace ServiceEvents.Web.Models;

public sealed record EmployeeEventViewModel(
    EventResponse Event,
    RegistrationStatus? RegistrationStatus,
    IReadOnlyCollection<EventPropertyValueViewModel> Properties);

public sealed record EventPropertyValueViewModel(
    string Name,
    string Value);

public sealed record EmployeeRegistrationViewModel(
    EventResponse Event,
    RegistrationStatus Status,
    DateTime RegisteredAt);
