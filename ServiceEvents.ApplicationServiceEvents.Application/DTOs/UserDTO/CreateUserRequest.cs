using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record CreateUserRequest(
    string FullName,
    string Department,
    string Position,
    UserRole Role);
}
