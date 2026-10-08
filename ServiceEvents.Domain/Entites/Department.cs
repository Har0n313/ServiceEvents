using ServiceEvents.Domain.Entites;

namespace ServiceEvents.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public ICollection<User> Users { get; private set; } = new List<User>();

    public ICollection<Event> Events { get; private set; } = new List<Event>();

    protected Department()
    {
    }

    public Department(string name)
    {
        Name = name;
    }

    public void Rename(string name)
    {
        Name = name;
    }
}
