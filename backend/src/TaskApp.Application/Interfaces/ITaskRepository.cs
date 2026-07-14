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
}
