namespace TaskApp.Application.DTOs;

/// <summary>
/// Read model for a task, returned by the API. DTOs decouple the wire contract
/// from the domain entity, so persistence or domain changes don't ripple into
/// API responses (and callers never see EF Core navigation/tracking concerns).
/// </summary>
/// <param name="Id">Unique identifier of the task.</param>
/// <param name="Title">Task title.</param>
/// <param name="IsCompleted">Whether the task is completed.</param>
/// <param name="Priority">Priority as text (Low/Medium/High).</param>
/// <param name="CreatedAt">UTC timestamp of when the task was created.</param>
/// <param name="CompletedAt">UTC timestamp of when the task was completed, or null while active.</param>
public record TaskDto(
    int Id,
    string Title,
    bool IsCompleted,
    string Priority,
    DateTime CreatedAt,
    DateTime? CompletedAt);
