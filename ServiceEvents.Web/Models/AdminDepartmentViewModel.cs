using System.ComponentModel.DataAnnotations;

namespace ServiceEvents.Web.Models;

public sealed class AdminDepartmentViewModel
{
    public Guid DepartmentId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
}
