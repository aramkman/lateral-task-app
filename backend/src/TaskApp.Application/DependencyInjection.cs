using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Services;
using TaskApp.Application.Validators;

namespace TaskApp.Application;

/// <summary>
/// Registers Application-layer services with the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds <see cref="TaskService"/> and all FluentValidation validators (discovered by
    /// assembly scan) to the container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TaskService>();
        services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();

        return services;
    }
}
