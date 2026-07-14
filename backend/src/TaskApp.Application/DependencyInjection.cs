using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Services;

namespace TaskApp.Application;

/// <summary>
/// Registers Application-layer services with the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Adds <see cref="TaskService"/> and other Application services to the container.</summary>
    /// <param name="services">The service collection to register into.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TaskService>();

        return services;
    }
}
