using ServiceEvents.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceEvents.Application.DTOs.EventRegistrationDTO
{
    public sealed record EventRegistrationResponse(
    Guid RegistrationId,
    Guid EventId,
    Guid UserId,
    DateTime RegisteredAt,
    RegistrationStatus Status);
}
