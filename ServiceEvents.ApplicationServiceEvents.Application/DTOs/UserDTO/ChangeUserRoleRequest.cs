using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record ChangeUserRoleRequest(
    Guid UserId,
    UserRole Role);
}
