using TaskApp.Application.DTOs;
using TaskApp.Domain.Entities;

namespace TaskApp.Application.Mapping;

/// <summary>
/// Manual entity-to-DTO mapping. A mapping library (AutoMapper) would be
/// disproportionate for a single, simple, one-directional mapping like this one.
/// </summary>
public static class TaskMappingExtensions
{
    /// <summary>Maps a <see cref="TaskItem"/> entity to its API read model.</summary>
    /// <param name="task">The entity to map.</param>
    public static TaskDto ToDto(this TaskItem task) =>
        new(task.Id, task.Title, task.IsCompleted, task.Priority.ToString(), task.CreatedAt, task.CompletedAt);
}
