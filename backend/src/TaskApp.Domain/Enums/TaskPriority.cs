namespace TaskApp.Domain.Enums;

/// <summary>
/// Relative importance of a <see cref="Entities.TaskItem"/>, used for display
/// ordering and as a filter dimension in the API and UI.
/// </summary>
public enum TaskPriority
{
    Low,
    Medium,
    High
}
