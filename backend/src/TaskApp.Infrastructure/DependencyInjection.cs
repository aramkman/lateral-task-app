using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Interfaces;
using TaskApp.Infrastructure.Persistence;
using TaskApp.Infrastructure.Repositories;

namespace TaskApp.Infrastructure;

/// <summary>
/// Registers Infrastructure-layer services (EF Core, SQLite) with the DI container.
/// Keeps the Api project unaware of which persistence provider is used underneath.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the SQLite-backed <see cref="TaskAppDbContext"/> and its repositories to the
    /// service collection, reading the connection string from configuration.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">App configuration, expected to contain a "Default" connection string.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<TaskAppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}
