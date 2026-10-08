using System.ComponentModel.DataAnnotations;

namespace ServiceEvents.Application.DTOs.EventDTO;

public sealed record CreateEventRequest(
    [Required]
    [StringLength(200)]
    string Title,
    [Required]
    string Description,
    [Required]
    DateTime StartDate,
    DateTime? EndDate,
    [StringLength(300)]
    string? Location,
    [Range(1, int.MaxValue)]
    int? MaxParticipants,
    [StringLength(500)]
    string? ImagePath);