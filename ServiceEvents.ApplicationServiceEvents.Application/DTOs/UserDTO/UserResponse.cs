using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record UserResponse(
    Guid UserId,
    string FullName,
    Guid DepartmentId,
    string Department,
    string Position,
    UserRole Role);
}
