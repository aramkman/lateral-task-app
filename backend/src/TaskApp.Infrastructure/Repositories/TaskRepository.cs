using Microsoft.EntityFrameworkCore;
using TaskApp.Application.Interfaces;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ITaskRepository"/>, backed by SQLite.
/// </summary>
public class TaskRepository : ITaskRepository
{
    #region Fields

    private readonly TaskAppDbContext _context;

    #endregion

    #region Constructor

    /// <summary>Creates the repository with the EF Core context, injected via DI.</summary>
    /// <param name="context">The database context to query and persist against.</param>
    public TaskRepository(TaskAppDbContext context)
    {
        _context = context;
    }

    #endregion

    #region Queries

    /// <inheritdoc />
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        // AsNoTracking: these entities are read-only for this call (mapped straight to
        // DTOs), so there's no need for EF Core's change-tracking overhead.
        return await _context.Tasks
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        // No AsNoTracking here: callers that fetch by id intend to mutate and save
        // the entity (e.g. toggling status), so it needs to stay tracked.
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    #endregion

    #region Commands

    /// <inheritdoc />
    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
