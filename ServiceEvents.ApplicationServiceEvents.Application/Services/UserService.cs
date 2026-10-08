using ServiceEvents.Application.DTOs.UserDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Mappings;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IDepartmentRepository departmentRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _eventRepository = eventRepository;
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
        var user = await _userRepository.GetWithDepartmentByIdAsync(
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

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Пользователь с таким email уже существует.");
        }

        var department = await GetDepartmentAsync(request.DepartmentId, cancellationToken);
        var user = new Domain.Entities.User(
            request.FullName,
            request.DepartmentId,
            request.Position,
            request.Email,
            request.Role);
        user.SetDepartment(department);
        user.SetPasswordHash(passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(
            id,
            cancellationToken);

        var department = await GetDepartmentAsync(request.DepartmentId, cancellationToken);
        user.UpdateInformation(
            request.FullName,
            request.DepartmentId,
            request.Position);
        user.SetDepartment(department);

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

        var organizedEvents = await _eventRepository.GetByOrganizerIdAsync(
            id,
            cancellationToken);
        if (organizedEvents.Count > 0)
        {
            throw new InvalidOperationException(
                $"Нельзя удалить пользователя «{user.FullName}»: он назначен организатором " +
                $"{organizedEvents.Count} мероприятий ({string.Join(", ", organizedEvents.Select(ev => ev.Title))}). " +
                "Сначала переназначьте или удалите эти мероприятия.");
        }

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
        var user = await _userRepository.GetWithDepartmentByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with id '{id}' was not found.");
        }

        return user;
    }

    private async Task<Domain.Entities.Department> GetDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        return await _departmentRepository.GetByIdAsync(departmentId, cancellationToken)
            ?? throw new InvalidOperationException("Выбранный департамент не найден.");
    }
}
