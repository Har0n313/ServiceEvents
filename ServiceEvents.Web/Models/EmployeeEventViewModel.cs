using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Models;

public sealed record EmployeeEventViewModel(
    EventResponse Event,
    RegistrationStatus? RegistrationStatus);

public sealed record EmployeeRegistrationViewModel(
    EventResponse Event,
    RegistrationStatus Status,
    DateTime RegisteredAt);
