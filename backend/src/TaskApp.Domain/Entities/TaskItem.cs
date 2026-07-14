using TaskApp.Domain.Enums;

namespace TaskApp.Domain.Entities;

/// <summary>
/// Represents a single task tracked by the application: its title, completion
/// state, priority, and the timestamps that record its lifecycle.
/// This is a plain POCO with no persistence attributes — EF Core mapping lives
/// in the Infrastructure layer, so Domain has no dependency on EF Core.
/// </summary>
public class TaskItem
{
    #region Properties

    /// <summary>Unique identifier of the task.</summary>
    public int Id { get; set; }

    /// <summary>Short description of the task. Required, max 200 characters.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Whether the task has been marked as completed.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Relative importance of the task.</summary>
    public TaskPriority Priority { get; set; }

    /// <summary>UTC timestamp of when the task was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of when the task was completed, or null while it is active.</summary>
    public DateTime? CompletedAt { get; set; }

    #endregion
}
