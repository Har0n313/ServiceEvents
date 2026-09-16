using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record UserResponse(
    Guid UserId,
    string FullName,
    string Department,
    string Position,
    UserRole Role);
}
