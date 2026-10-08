using Microsoft.EntityFrameworkCore;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Domain.Enums;
using ServiceEvents.Infrastructure.EntityFramework;

namespace ServiceEvents.Infrastructure.Repositories;

public class UserRepository
    : Repository<User>, IUserRepository
{
    public UserRepository(ServiceEventsDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(user => user.Department)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(user => user.Department)
            .Where(x => x.Role == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(user => user.Department)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public Task<User?> GetWithDepartmentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(user => user.Department)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }
}