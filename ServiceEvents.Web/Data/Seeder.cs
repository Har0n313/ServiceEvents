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
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();

        var demoUsers = new[]
        {
            (Email: "admin@localhost", FullName: "Администратор", Position: "Администратор", Role: UserRole.Admin, Password: "Admin123!"),
            (Email: "organizer@localhost", FullName: "Тестовый организатор", Position: "Организатор", Role: UserRole.Organizer, Password: "Organizer123!"),
            (Email: "employee@localhost", FullName: "Тестовый сотрудник", Position: "Сотрудник", Role: UserRole.Employee, Password: "Employee123!")
        };

        var changed = false;
        foreach (var demoUser in demoUsers)
        {
            var existingUser = await userRepo.GetByEmailAsync(demoUser.Email, cancellationToken);
            if (existingUser is not null)
            {
                if (string.IsNullOrEmpty(existingUser.PasswordHash))
                {
                    existingUser.SetPasswordHash(hasher.HashPassword(existingUser, demoUser.Password));
                    await userRepo.UpdateAsync(existingUser, cancellationToken);
                    changed = true;
                }

                continue;
            }

            var user = new User(
                demoUser.FullName,
                "Информационные технологии",
                demoUser.Position,
                demoUser.Email,
                demoUser.Role);
            user.SetPasswordHash(hasher.HashPassword(user, demoUser.Password));
            await userRepo.AddAsync(user, cancellationToken);
            changed = true;
        }

        if (changed)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
