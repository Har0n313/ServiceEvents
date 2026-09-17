using ServiceEvents.Domain.Entites;

namespace ServiceEvents.Domain.Entities;

public class EventPropertyValue : BaseEntity
{
    public Guid EventId { get; private set; }

    public Guid PropertyId { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public Event Event { get; private set; } = null!;

    public EventProperty Property { get; private set; } = null!;

    protected EventPropertyValue()
    {
    }

    public EventPropertyValue(
        Guid eventId,
        Guid propertyId,
        string value)
    {
        EventId = eventId;
        PropertyId = propertyId;
        Value = value;
    }
    public void UpdateValue(string value)
    {
        Value = value;
    }

    public void ChangeValue(string value)
    {
        Value = value;
    }
}