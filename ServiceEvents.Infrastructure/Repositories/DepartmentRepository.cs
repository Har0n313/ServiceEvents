using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public sealed class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<Department>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Department?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            department => department.Name.ToLower() == name.ToLower(),
            cancellationToken);
    }
}
