using ServiceEvents.Domain.Entites;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? PasswordHash { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Department Department { get; private set; } = null!;

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
        Guid departmentId,
        string position,
        string email,
        UserRole role = UserRole.Employee)
    {
        FullName = fullName;
        DepartmentId = departmentId;
        Position = position;
        Email = email;
        Role = role;
    }

    public void UpdateInformation(
        string fullName,
        Guid departmentId,
        string position)
    {
        FullName = fullName;
        DepartmentId = departmentId;
        Position = position;
    }

    public void SetDepartment(Department department)
    {
        Department = department;
        DepartmentId = department.Id;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void SetPasswordHash(string hash)
    {
        PasswordHash = hash;
    }
}