using TaskApp.Domain.Entities;

namespace TaskApp.Application.Interfaces;

/// <summary>
/// Persistence abstraction for <see cref="TaskItem"/>. Application and Api depend
/// on this interface, never on the concrete EF Core implementation — the concrete
/// repository is injected by Infrastructure at composition time.
/// </summary>
public interface ITaskRepository
{
    /// <summary>Returns every task in the store, unfiltered.</summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new task. <paramref name="task"/>.Id is populated by the store
    /// (auto-increment) once the operation completes.
    /// </summary>
    /// <param name="task">The task to persist.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);
}
