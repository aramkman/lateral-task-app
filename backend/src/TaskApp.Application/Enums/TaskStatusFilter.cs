namespace TaskApp.Application.Enums;

/// <summary>
/// Status filter accepted by the list-tasks endpoint. This is a query/API concept,
/// not a domain one — the domain only knows <c>IsCompleted</c>; the grouping into
/// all/active/completed exists purely to serve the list endpoint and the UI filters.
/// </summary>
public enum TaskStatusFilter
{
    All,
    Active,
    Completed
}
