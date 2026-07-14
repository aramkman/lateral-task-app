using Microsoft.AspNetCore.Mvc;
using TaskApp.Application.Common;
using TaskApp.Application.DTOs;
using TaskApp.Application.Enums;
using TaskApp.Application.Services;

namespace TaskApp.Api.Controllers;

/// <summary>
/// HTTP endpoints for managing tasks. Owns request/response concerns only —
/// business logic lives in <see cref="TaskService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    #region Fields

    private readonly TaskService _taskService;

    #endregion

    #region Constructor

    /// <summary>Creates the controller with its Application-layer service, injected via DI.</summary>
    /// <param name="taskService">Service that owns task business logic.</param>
    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    #endregion

    #region Endpoints

    /// <summary>
    /// Lists tasks, optionally filtered by status.
    /// </summary>
    /// <param name="status">Status filter: all, active, or completed. Defaults to all.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if the client disconnects.</param>
    /// <returns>200 with the matching tasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetTasks(
        [FromQuery] TaskStatusFilter status = TaskStatusFilter.All,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.GetTasksAsync(status, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">Title and priority of the task to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if the client disconnects.</param>
    /// <returns>201 with the created task, or 400 with validation errors.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateTask(
        [FromBody] CreateTaskDto request,
        CancellationToken cancellationToken)
    {
        var result = await _taskService.CreateTaskAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return ValidationProblem(ModelState);
        }

        // No dedicated GET-by-id endpoint exists yet (optional per spec), so the
        // Location header points at the conventional resource URI rather than a live route.
        return Created($"/api/tasks/{result.Value!.Id}", result.Value);
    }

    /// <summary>
    /// Toggles a task's completed status.
    /// </summary>
    /// <param name="id">Id of the task to toggle.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if the client disconnects.</param>
    /// <returns>200 with the updated task, or 404 if it doesn't exist.</returns>
    [HttpPatch("{id}/toggle")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> ToggleTaskStatus(int id, CancellationToken cancellationToken)
    {
        var result = await _taskService.ToggleTaskStatusAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Problem(title: result.Errors[0], statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }

    #endregion
}
