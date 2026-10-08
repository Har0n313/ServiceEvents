using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Models;

public sealed record OrganizerParticipantViewModel(
    string FullName,
    DateTime RegisteredAt,
    RegistrationStatus Status);

public sealed record OrganizerParticipantsViewModel(
    EventResponse Event,
    IReadOnlyCollection<OrganizerParticipantViewModel> Participants);
