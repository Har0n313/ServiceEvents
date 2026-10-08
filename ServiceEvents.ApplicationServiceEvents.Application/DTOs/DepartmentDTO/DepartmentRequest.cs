using System.ComponentModel.DataAnnotations;

namespace ServiceEvents.Application.DTOs.DepartmentDTO;

public sealed record DepartmentRequest(
    [param: Required]
    [param: StringLength(200)]
    string Name);
