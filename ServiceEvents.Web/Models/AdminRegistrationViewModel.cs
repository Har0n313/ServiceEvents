using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Models;

public sealed record AdminRegistrationViewModel(
    string EventTitle,
    string UserName,
    DateTime RegisteredAt,
    RegistrationStatus Status);
