using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;

namespace TaskApp.Infrastructure.Persistence;

/// <summary>
/// Applies pending migrations and populates the database with example tasks on
/// first run, so the app shows a populated list immediately instead of an empty state.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Migrates the database to the latest schema, then seeds example tasks only if
    /// the table is empty — this keeps the seed idempotent across app restarts.
    /// </summary>
    /// <param name="context">The database context to migrate and seed.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public static async Task SeedAsync(TaskAppDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (await context.Tasks.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Review the quarterly design proposal",
                Priority = TaskPriority.High,
                IsCompleted = false,
                CreatedAt = now.AddDays(-2)
            },
            new TaskItem
            {
                Title = "Reply to Sofia about the launch date",
                Priority = TaskPriority.Medium,
                IsCompleted = false,
                CreatedAt = now.AddDays(-1)
            },
            new TaskItem
            {
                Title = "Water the plants",
                Priority = TaskPriority.Low,
                IsCompleted = true,
                CreatedAt = now.AddDays(-3),
                CompletedAt = now.AddDays(-3).AddHours(2)
            },
            new TaskItem
            {
                Title = "Prepare demo environment for the walkthrough",
                Priority = TaskPriority.High,
                IsCompleted = false,
                CreatedAt = now
            }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
