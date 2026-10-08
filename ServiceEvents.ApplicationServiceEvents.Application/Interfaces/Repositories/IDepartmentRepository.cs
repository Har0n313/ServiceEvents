using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IReadOnlyCollection<Department>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Department?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}
