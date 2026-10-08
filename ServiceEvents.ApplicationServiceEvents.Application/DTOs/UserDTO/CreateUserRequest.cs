using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record CreateUserRequest(
        string Email,
        string FullName,
        string Department,
        string Position,
        UserRole Role);
}
