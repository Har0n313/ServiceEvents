using ServiceEvents.Domain.Entites;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;

    public string Department { get; private set; } = string.Empty;

    public string Position { get; private set; } = string.Empty;

    public UserRole Role { get; private set; } = UserRole.Employee;

    public ICollection<Event> OrganizedEvents { get; private set; }
        = new List<Event>();

    public ICollection<EventRegistration> Registrations { get; private set; }
        = new List<EventRegistration>();

    protected User()
    {
    }

    public User(
        string fullName,
        string department,
        string position,
        UserRole role = UserRole.Employee)
    {
        FullName = fullName;
        Department = department;
        Position = position;
        Role = role;
    }

    public void UpdateInformation(
        string fullName,
        string department,
        string position)
    {
        FullName = fullName;
        Department = department;
        Position = position;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }
}