using Microsoft.AspNetCore.Mvc;
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

    #endregion
}
