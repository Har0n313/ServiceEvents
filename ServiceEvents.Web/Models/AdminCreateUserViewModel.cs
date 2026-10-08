using System.ComponentModel.DataAnnotations;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Models;

public sealed class AdminCreateUserViewModel
{
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Position { get; set; } = string.Empty;

    [EnumDataType(typeof(UserRole))]
    public UserRole Role { get; set; } = UserRole.Employee;
}
