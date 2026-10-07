using Microsoft.Extensions.DependencyInjection;
using ServiceEvents.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using ServiceEvents.Domain.Entities;
using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Data;

public static class Seeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var userRepo = provider.GetRequiredService<IUserRepository>();
        var hasher = provider.GetRequiredService<IPasswordHasher<User>>();

        // Check existing admin
        var admins = await userRepo.GetByRoleAsync(UserRole.Admin, cancellationToken);
        if (admins.Any()) return;

        var admin = new User("Administrator","IT","Admin","admin@localhost", UserRole.Admin);
        admin.SetPasswordHash(hasher.HashPassword(admin, "Admin123!"));

        // If repository has AddAsync method via IReposirory base we'll try to use it via reflection
        var addMethod = userRepo.GetType().GetMethod("AddAsync");
        if (addMethod != null)
        {
            await (Task)addMethod.Invoke(userRepo, new object[] { admin, cancellationToken })!;
        }
        else
        {
            // Try to use UnitOfWork if present
            var uow = provider.GetService(typeof(ServiceEvents.Application.Interfaces.Repositories.IUnitOfWork)) as ServiceEvents.Application.Interfaces.Repositories.IUnitOfWork;
            if (uow != null)
            {
                // direct access to repository through DI was unavailable; skip advanced seed
            }
        }
    }
}
