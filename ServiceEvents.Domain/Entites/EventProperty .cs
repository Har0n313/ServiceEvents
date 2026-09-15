using ServiceEvents.Domain.Entites;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Domain.Entities;

public class EventProperty : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public PropertyDataType DataType { get; private set; }

    public bool IsActive { get; private set; } = true;

    public ICollection<EventPropertyValue> Values { get; private set; }
        = new List<EventPropertyValue>();

    protected EventProperty()
    {
    }

    public EventProperty(
        string name,
        PropertyDataType dataType)
    {
        Name = name;
        DataType = dataType;
    }

    public void Update(string name, PropertyDataType dataType)
    {
        Name = name;
        DataType = dataType;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}