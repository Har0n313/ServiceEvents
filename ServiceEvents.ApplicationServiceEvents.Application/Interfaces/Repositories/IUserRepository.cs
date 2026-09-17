using ServiceEvents.Domain.Entities;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<IReadOnlyCollection<User>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<User>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default);
}