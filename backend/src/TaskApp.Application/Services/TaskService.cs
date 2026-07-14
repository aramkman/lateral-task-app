using TaskApp.Application.DTOs;
using TaskApp.Application.Enums;
using TaskApp.Application.Interfaces;
using TaskApp.Application.Mapping;

namespace TaskApp.Application.Services;

/// <summary>
/// Owns task business logic: orchestrates the repository and applies rules
/// (filtering, status transitions) that don't belong in persistence or HTTP.
/// </summary>
public class TaskService
{
    #region Fields

    private readonly ITaskRepository _repository;

    #endregion

    #region Constructor

    /// <summary>Creates the service with its repository dependency, injected via DI.</summary>
    /// <param name="repository">Abstraction over task persistence.</param>
    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    #endregion

    #region Queries

    /// <summary>
    /// Returns tasks matching the given status filter, mapped to their API read model.
    /// Filtering happens in-memory over the full set returned by the repository: at this
    /// scope (a personal task list) that's simpler and keeps the business rule unit-testable
    /// without a database. At a much larger scale this would move into the repository query.
    /// </summary>
    /// <param name="filter">Which subset of tasks to return.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(TaskStatusFilter filter, CancellationToken cancellationToken)
    {
        var tasks = await _repository.GetAllAsync(cancellationToken);

        var filtered = filter switch
        {
            TaskStatusFilter.Active => tasks.Where(t => !t.IsCompleted),
            TaskStatusFilter.Completed => tasks.Where(t => t.IsCompleted),
            _ => tasks
        };

        return filtered.Select(t => t.ToDto()).ToList();
    }

    #endregion
}
