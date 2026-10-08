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
        var departmentRepo = provider.GetRequiredService<IDepartmentRepository>();
        var eventRepo = provider.GetRequiredService<IEventRepository>();
        var propertyRepo = provider.GetRequiredService<IEventPropertyRepository>();
        var propertyValueRepo = provider.GetRequiredService<IEventPropertyValueRepository>();
        var hasher = provider.GetRequiredService<IPasswordHasher<User>>();
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();

        const string demoDepartmentName = "Информационные технологии";
        var department = await departmentRepo.GetByNameAsync(demoDepartmentName, cancellationToken);
        var changed = false;
        if (department is null)
        {
            department = new Department(demoDepartmentName);
            await departmentRepo.AddAsync(department, cancellationToken);
            changed = true;
        }

        var demoUsers = new[]
        {
            (Email: "admin@gmail.com", FullName: "Администратор", Position: "Администратор", Role: UserRole.Admin, Password: "Admin123!"),
            (Email: "organizer@localhost", FullName: "Тестовый организатор", Position: "Организатор", Role: UserRole.Organizer, Password: "Organizer123!"),
            (Email: "employee@localhost", FullName: "Тестовый сотрудник", Position: "Сотрудник", Role: UserRole.Employee, Password: "Employee123!")
        };

        User? organizer = null;
        foreach (var demoUser in demoUsers)
        {
            var existingUser = await userRepo.GetByEmailAsync(demoUser.Email, cancellationToken);
            if (existingUser is not null)
            {
                var passwordResult = string.IsNullOrEmpty(existingUser.PasswordHash)
                    ? PasswordVerificationResult.Failed
                    : hasher.VerifyHashedPassword(
                        existingUser,
                        existingUser.PasswordHash,
                        demoUser.Password);
                var userChanged = false;

                if (existingUser.FullName != demoUser.FullName
                    || existingUser.DepartmentId != department.Id
                    || existingUser.Position != demoUser.Position)
                {
                    existingUser.UpdateInformation(
                        demoUser.FullName,
                        department.Id,
                        demoUser.Position);
                    existingUser.SetDepartment(department);
                    userChanged = true;
                }

                if (existingUser.Role != demoUser.Role)
                {
                    existingUser.ChangeRole(demoUser.Role);
                    userChanged = true;
                }

                if (passwordResult == PasswordVerificationResult.Failed)
                {
                    existingUser.SetPasswordHash(hasher.HashPassword(existingUser, demoUser.Password));
                    userChanged = true;
                }

                if (userChanged)
                {
                    await userRepo.UpdateAsync(existingUser, cancellationToken);
                    changed = true;
                }

                if (demoUser.Email == "organizer@localhost")
                {
                    organizer = existingUser;
                }

                continue;
            }

            var user = new User(
                demoUser.FullName,
                department.Id,
                demoUser.Position,
                demoUser.Email,
                demoUser.Role);
            user.SetDepartment(department);
            user.SetPasswordHash(hasher.HashPassword(user, demoUser.Password));
            await userRepo.AddAsync(user, cancellationToken);
            if (demoUser.Email == "organizer@localhost")
            {
                organizer = user;
            }
            changed = true;
        }

        if (organizer is null)
        {
            organizer = await userRepo.GetByEmailAsync("organizer@localhost", cancellationToken);
        }

        if (organizer is null)
        {
            throw new InvalidOperationException("Не удалось создать тестового организатора.");
        }
        var (sampleProperties, propertiesChanged) = await SeedPropertiesAsync(propertyRepo, cancellationToken);
        changed |= propertiesChanged;
        var existingEvents = await eventRepo.GetAllAsync(cancellationToken);
        var demoEvents = new[]
        {
            new DemoEvent(
                "Завтрак команды",
                "Неформально начнём день за завтраком, познакомимся поближе и обсудим идеи.",
                7,
                "Кафе «Утро»",
                16,
                new Dictionary<string, string>
                {
                    ["Можно с детьми"] = "Да",
                    ["Можно со своим алкоголем"] = "Нет",
                    ["Дресс-код"] = "Повседневный"
                }),
            new DemoEvent(
                "Вечер настольных игр",
                "Командные и настольные игры, лёгкие закуски и хорошее настроение.",
                14,
                "Комната отдыха, офис",
                20,
                new Dictionary<string, string>
                {
                    ["Можно с детьми"] = "Да",
                    ["Можно со своим алкоголем"] = "Нет",
                    ["Дресс-код"] = "Свободный"
                }),
            new DemoEvent(
                "Прогулка по парку",
                "Встречаемся на прогулку, чтобы отдохнуть, пообщаться и провести время вместе.",
                21,
                "Центральный парк",
                25,
                new Dictionary<string, string>
                {
                    ["Можно с детьми"] = "Да",
                    ["Можно со своим алкоголем"] = "Нет",
                    ["Дресс-код"] = "Удобная одежда"
                })
        };

        foreach (var demoEvent in demoEvents)
        {
            var eventItem = existingEvents.FirstOrDefault(
                existing => existing.Title == demoEvent.Title
                    && existing.OrganizerId == organizer.Id
                    && existing.DepartmentId == department.Id);
            if (eventItem is null)
            {
                var startDate = DateTime.UtcNow.Date.AddDays(demoEvent.DaysFromToday).AddHours(9);
                var newEvent = new Event(
                    demoEvent.Title,
                    demoEvent.Description,
                    startDate,
                    organizer.Id,
                    department.Id,
                    demoEvent.Location,
                    startDate.AddHours(2),
                    demoEvent.MaxParticipants);
                newEvent.Publish();
                await eventRepo.AddAsync(newEvent, cancellationToken);
                eventItem = newEvent;
                existingEvents = existingEvents.Append(newEvent).ToList();
                changed = true;
            }

            foreach (var (propertyName, value) in demoEvent.PropertyValues)
            {
                var property = sampleProperties[propertyName];
                if (await propertyValueRepo.GetAsync(eventItem.Id, property.Id, cancellationToken) is not null)
                {
                    continue;
                }

                await propertyValueRepo.AddAsync(
                    new EventPropertyValue(eventItem.Id, property.Id, value),
                    cancellationToken);
                changed = true;
            }
        }

        if (changed)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task<(Dictionary<string, EventProperty> Properties, bool Changed)> SeedPropertiesAsync(
        IEventPropertyRepository propertyRepository,
        CancellationToken cancellationToken)
    {
        var requiredProperties = new[]
        {
            (Name: "Можно с детьми", DataType: PropertyDataType.Boolean),
            (Name: "Можно со своим алкоголем", DataType: PropertyDataType.Boolean),
            (Name: "Дресс-код", DataType: PropertyDataType.String)
        };
        var existingProperties = await propertyRepository.GetAllAsync(cancellationToken);
        var result = new Dictionary<string, EventProperty>(StringComparer.OrdinalIgnoreCase);
        var changed = false;

        foreach (var required in requiredProperties)
        {
            var existing = existingProperties.FirstOrDefault(
                property => string.Equals(property.Name, required.Name, StringComparison.OrdinalIgnoreCase));
            if (existing is not null && existing.DataType != required.DataType)
            {
                throw new InvalidOperationException(
                    $"Характеристика «{required.Name}» уже существует с другим типом данных.");
            }

            if (existing is null)
            {
                existing = new EventProperty(required.Name, required.DataType);
                await propertyRepository.AddAsync(existing, cancellationToken);
                existingProperties = existingProperties.Append(existing).ToList();
                changed = true;
            }
            else if (!existing.IsActive)
            {
                existing.Activate();
                await propertyRepository.UpdateAsync(existing, cancellationToken);
                changed = true;
            }

            result.Add(required.Name, existing);
        }

        return (result, changed);
    }

    private sealed record DemoEvent(
        string Title,
        string Description,
        int DaysFromToday,
        string Location,
        int MaxParticipants,
        IReadOnlyDictionary<string, string> PropertyValues);
}
