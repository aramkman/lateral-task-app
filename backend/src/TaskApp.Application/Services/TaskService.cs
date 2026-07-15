using FluentValidation;
using TaskApp.Application.Common;
using TaskApp.Application.DTOs;
using TaskApp.Application.Enums;
using TaskApp.Application.Interfaces;
using TaskApp.Application.Mapping;
using TaskApp.Domain.Entities;

namespace TaskApp.Application.Services;

/// <summary>
/// Owns task business logic: orchestrates the repository and applies rules
/// (filtering, status transitions) that don't belong in persistence or HTTP.
/// </summary>
public class TaskService
{
    #region Fields

    private readonly ITaskRepository _repository;
    private readonly IValidator<CreateTaskDto> _createTaskValidator;

    #endregion

    #region Constructor

    /// <summary>Creates the service with its dependencies, injected via DI.</summary>
    /// <param name="repository">Abstraction over task persistence.</param>
    /// <param name="createTaskValidator">Validates task creation requests.</param>
    public TaskService(ITaskRepository repository, IValidator<CreateTaskDto> createTaskValidator)
    {
        _repository = repository;
        _createTaskValidator = createTaskValidator;
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

    #region Commands

    /// <summary>
    /// Validates and creates a new task. The repository is only called when validation
    /// passes — on failure this returns the validation errors without touching persistence.
    /// </summary>
    /// <param name="request">The task to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<ServiceResult<TaskDto>> CreateTaskAsync(CreateTaskDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _createTaskValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ServiceResult<TaskDto>.ValidationFailure(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Priority = request.Priority,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(task, cancellationToken);

        return ServiceResult<TaskDto>.Success(task.ToDto());
    }

    /// <summary>
    /// Flips a task's completed status. Sets <c>CompletedAt</c> when marking it done,
    /// clears it when reopening it.
    /// </summary>
    /// <param name="id">Id of the task to toggle.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<ServiceResult<TaskDto>> ToggleTaskStatusAsync(int id, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return ServiceResult<TaskDto>.NotFound($"Task {id} was not found.");
        }

        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;

        await _repository.UpdateAsync(task, cancellationToken);

        return ServiceResult<TaskDto>.Success(task.ToDto());
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">Id of the task to delete.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<ServiceResult> DeleteTaskAsync(int id, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return ServiceResult.NotFound($"Task {id} was not found.");
        }

        await _repository.DeleteAsync(task, cancellationToken);

        return ServiceResult.Success();
    }

    #endregion
}
