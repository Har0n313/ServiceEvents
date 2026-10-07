using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Application.Services;
using ServiceEvents.Infrastructure.EntityFramework;
using ServiceEvents.Infrastructure.Repositories;

namespace ServiceEvents.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ServiceEventsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
        services.AddScoped<IEventPropertyRepository, EventPropertyRepository>();
        services.AddScoped<IEventPropertyValueRepository, EventPropertyValueRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IEventRegistrationService, EventRegistrationService>();
        services.AddScoped<IEventPropertyService, EventPropertyService>();
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}