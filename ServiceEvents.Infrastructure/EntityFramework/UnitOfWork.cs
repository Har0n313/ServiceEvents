using ServiceEvents.Application.Interfaces.Repositories;

namespace ServiceEvents.Infrastructure.EntityFramework;

public class UnitOfWork : IUnitOfWork
{
    private readonly ServiceEventsDbContext _context;

    public UnitOfWork(ServiceEventsDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}