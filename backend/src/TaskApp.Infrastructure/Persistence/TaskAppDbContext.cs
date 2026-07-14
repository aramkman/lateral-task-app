using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the task management application.
/// Owns the SQLite connection and entity-to-table mappings.
/// </summary>
public class TaskAppDbContext : DbContext
{
    #region Constructor

    /// <summary>
    /// Creates the context with options supplied by dependency injection
    /// (provider, connection string), configured in the Api composition root.
    /// </summary>
    /// <param name="options">EF Core options, including the SQLite connection string.</param>
    public TaskAppDbContext(DbContextOptions<TaskAppDbContext> options) : base(options)
    {
    }

    #endregion

    #region DbSets

    /// <summary>Tasks tracked by the application.</summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    #endregion

    #region Model configuration

    /// <summary>
    /// Applies all <see cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}"/>
    /// classes found in this assembly, keeping entity mapping out of the DbContext itself.
    /// </summary>
    /// <param name="modelBuilder">Builder used to construct the EF Core model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskAppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    #endregion
}
