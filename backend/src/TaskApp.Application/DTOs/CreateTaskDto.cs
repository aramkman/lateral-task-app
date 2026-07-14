using TaskApp.Domain.Enums;

namespace TaskApp.Application.DTOs;

/// <summary>
/// Request body for creating a task. Reuses the Domain <see cref="TaskPriority"/>
/// enum directly — it's a plain value type with no persistence/behavior baggage,
/// so referencing it here doesn't leak entity or EF Core concerns into the DTO.
/// </summary>
/// <param name="Title">Task title. Required, max 200 characters (enforced by <c>CreateTaskDtoValidator</c>).</param>
/// <param name="Priority">Priority of the new task.</param>
public record CreateTaskDto(string Title, TaskPriority Priority);
