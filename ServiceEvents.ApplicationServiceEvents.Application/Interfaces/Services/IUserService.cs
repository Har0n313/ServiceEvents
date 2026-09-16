using ServiceEvents.Application.DTOs.UserDTO;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserResponse>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    Task ChangeRoleAsync(
        Guid id,
        UserRole role,
        CancellationToken cancellationToken = default);
}