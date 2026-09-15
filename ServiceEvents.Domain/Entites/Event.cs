using ServiceEvents.Domain.Entites;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Domain.Entities;

public class Event : BaseEntity
{
    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public string? Location { get; private set; }

    public int? MaxParticipants { get; private set; }

    public string? ImagePath { get; private set; }

    public EventStatus Status { get; private set; } = EventStatus.Draft;

    public Guid OrganizerId { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public User Organizer { get; private set; } = null!;

    public ICollection<EventRegistration> Registrations { get; private set; }
        = new List<EventRegistration>();

    public ICollection<EventPropertyValue> PropertyValues { get; private set; }
        = new List<EventPropertyValue>();

    protected Event()
    {
    }

    public Event(
        string title,
        string description,
        DateTime startDate,
        Guid organizerId,
        string? location = null,
        DateTime? endDate = null,
        int? maxParticipants = null,
        string? imagePath = null)
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        OrganizerId = organizerId;
        Location = location;
        EndDate = endDate;
        MaxParticipants = maxParticipants;
        ImagePath = imagePath;
    }

    public void UpdateInformation(
        string title,
        string description,
        DateTime startDate,
        string? location,
        DateTime? endDate,
        int? maxParticipants,
        string? imagePath)
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        Location = location;
        EndDate = endDate;
        MaxParticipants = maxParticipants;
        ImagePath = imagePath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        Status = EventStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseRegistration()
    {
        Status = EventStatus.RegistrationClosed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = EventStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}