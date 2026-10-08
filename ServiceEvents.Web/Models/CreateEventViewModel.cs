using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ServiceEvents.Application.DTOs.EventDTO;

namespace ServiceEvents.Web.Models;

public sealed class CreateEventViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string StartDate { get; set; } = string.Empty;

    public string? EndDate { get; set; }

    [StringLength(300)]
    public string? Location { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxParticipants { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public static CreateEventViewModel FromRequest(CreateEventRequest request)
    {
        return new CreateEventViewModel
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
            EndDate = request.EndDate?.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
            Location = request.Location,
            MaxParticipants = request.MaxParticipants,
            ImagePath = request.ImagePath
        };
    }

    public bool TryParseDates(out DateTime startDate, out DateTime? endDate)
    {
        var startParsed = DateTime.TryParseExact(
            StartDate,
            "dd.MM.yyyy HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out startDate);

        endDate = null;
        if (string.IsNullOrWhiteSpace(EndDate))
        {
            return startParsed;
        }

        var endParsed = DateTime.TryParseExact(
            EndDate,
            "dd.MM.yyyy HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedEndDate);

        if (endParsed)
        {
            endDate = parsedEndDate;
        }

        return startParsed && endParsed;
    }

    public CreateEventRequest ToRequest(DateTime startDate, DateTime? endDate)
    {
        return new CreateEventRequest(
            Title,
            Description,
            startDate,
            endDate,
            Location,
            MaxParticipants,
            ImagePath);
    }
}
