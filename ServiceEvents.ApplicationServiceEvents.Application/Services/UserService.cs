using ServiceEvents.Application.DTOs.UserDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Mappings;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(
            cancellationToken);

        return users
            .Select(user => user.ToResponse())
            .ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            id,
            cancellationToken);

        return user?.ToResponse();
    }

    public async Task<IReadOnlyCollection<UserResponse>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByRoleAsync(
            role,
            cancellationToken);

        return users
            .Select(user => user.ToResponse())
            .ToList();
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(
            id,
            cancellationToken);

        user.UpdateInformation(
            request.FullName,
            request.Department,
            request.Position);

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ChangeRoleAsync(
        Guid id,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(
            id,
            cancellationToken);

        user.ChangeRole(role);

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(
            id,
            cancellationToken);

        await _userRepository.DeleteAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Domain.Entities.User> GetUserAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with id '{id}' was not found.");
        }

        return user;
    }
}
