using ServiceEvents.Application.DTOs.UserDTO;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Mappings;

public static class UserProfile
{
    public static UserResponse ToResponse(this Domain.Entities.User entity)
    {
        return new UserResponse(
            entity.Id,
            entity.FullName,
            entity.Department,
            entity.Position,
            entity.Role);
    }
}