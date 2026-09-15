using ServiceEvents.Domain.Entites;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Domain.Entities;

public class EventRegistration : BaseEntity
{
    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;

    public RegistrationStatus Status { get; private set; }
        = RegistrationStatus.Active;

    public Event Event { get; private set; } = null!;

    public User User { get; private set; } = null!;

    protected EventRegistration()
    {
    }

    public EventRegistration(Guid eventId, Guid userId)
    {
        EventId = eventId;
        UserId = userId;
    }

    public void Cancel()
    {
        Status = RegistrationStatus.Cancelled;
    }

    public void Restore()
    {
        Status = RegistrationStatus.Active;
    }
}