using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.EventRegistrationDTO
{
    public sealed record UpdateEventRegistrationRequest(
    Guid RegistrationId,
    RegistrationStatus Status);
}
