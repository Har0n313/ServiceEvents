using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace ServiceEvents.Infrastructure.EntityFramework;

public sealed class ServiceEventsDbContextFactory
    : IDesignTimeDbContextFactory<ServiceEventsDbContext>
{
    public ServiceEventsDbContext CreateDbContext(string[] args)
    {
        var contentRoot = FindWebContentRoot();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";
        var connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? ReadConnectionString(Path.Combine(contentRoot, $"appsettings.{environment}.json"))
            ?? ReadConnectionString(Path.Combine(contentRoot, "appsettings.json"));

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection must be configured in the web app settings or environment.");
        }

        var options = new DbContextOptionsBuilder<ServiceEventsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ServiceEventsDbContext(options);
    }

    private static string? ReadConnectionString(string settingsPath)
    {
        if (!File.Exists(settingsPath))
        {
            return null;
        }

        using var settings = JsonDocument.Parse(File.ReadAllText(settingsPath));
        return settings.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
            && connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection)
                ? defaultConnection.GetString()
                : null;
    }

    private static string FindWebContentRoot()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (var directory = currentDirectory; directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }

            var webProjectDirectory = Path.Combine(directory.FullName, "ServiceEvents.Web");
            if (File.Exists(Path.Combine(webProjectDirectory, "appsettings.json")))
            {
                return webProjectDirectory;
            }
        }

        throw new InvalidOperationException(
            "Could not locate ServiceEvents.Web appsettings.json from the current directory.");
    }
}
