using Moq;
using TaskApp.Application.Common;
using TaskApp.Application.DTOs;
using TaskApp.Application.Enums;
using TaskApp.Application.Interfaces;
using TaskApp.Application.Services;
using TaskApp.Application.Validators;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;

namespace TaskApp.Tests.Services;

/// <summary>
/// Unit tests for <see cref="TaskService"/>. The repository is mocked; the real
/// <see cref="CreateTaskDtoValidator"/> is used so these tests exercise the actual
/// validation rules, not a stand-in for them.
/// </summary>
public class TaskServiceTests
{
    #region Fixture

    private readonly Mock<ITaskRepository> _repositoryMock = new();
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _sut = new TaskService(_repositoryMock.Object, new CreateTaskDtoValidator());
    }

    #endregion

    #region GetTasksAsync

    [Fact]
    public async Task GetTasks_WithStatusFilter_ReturnsOnlyMatchingTasks()
    {
        // Arrange: a mixed set of active and completed tasks in the mocked repository.
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Active task A", Priority = TaskPriority.Low, IsCompleted = false },
            new() { Id = 2, Title = "Active task B", Priority = TaskPriority.Medium, IsCompleted = false },
            new() { Id = 3, Title = "Completed task", Priority = TaskPriority.High, IsCompleted = true }
        };
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act & Assert: all
        var all = await _sut.GetTasksAsync(TaskStatusFilter.All, CancellationToken.None);
        Assert.Equal(3, all.Count);

        // Act & Assert: active
        var active = await _sut.GetTasksAsync(TaskStatusFilter.Active, CancellationToken.None);
        Assert.Equal(2, active.Count);
        Assert.All(active, t => Assert.False(t.IsCompleted));

        // Act & Assert: completed
        var completed = await _sut.GetTasksAsync(TaskStatusFilter.Completed, CancellationToken.None);
        Assert.Single(completed);
        Assert.True(completed[0].IsCompleted);
    }

    #endregion

    #region CreateTaskAsync

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var request = new CreateTaskDto("Write project README", TaskPriority.Medium);
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((task, _) => task.Id = 42)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateTaskAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(42, result.Value.Id);
        Assert.Equal("Write project README", result.Value.Title);
        Assert.Equal("Medium", result.Value.Priority);
        Assert.False(result.Value.IsCompleted);
        Assert.Null(result.Value.CompletedAt);
        Assert.InRange(result.Value.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTask_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var request = new CreateTaskDto(string.Empty, TaskPriority.Low);

        // Act
        var result = await _sut.CreateTaskAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateTask_WithTitleExceedingMaxLength_FailsValidation()
    {
        // Arrange
        var request = new CreateTaskDto(new string('a', 201), TaskPriority.Low);

        // Act
        var result = await _sut.CreateTaskAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region ToggleTaskStatusAsync

    [Fact]
    public async Task ToggleStatus_WhenTaskExists_FlipsIsCompletedAndSetsCompletedAt()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 7,
            Title = "Water the plants",
            Priority = TaskPriority.Low,
            IsCompleted = false,
            CompletedAt = null
        };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act: toggle once (active -> completed)
        var completedResult = await _sut.ToggleTaskStatusAsync(7, CancellationToken.None);

        // Assert
        Assert.True(completedResult.IsSuccess);
        Assert.True(completedResult.Value!.IsCompleted);
        Assert.NotNull(completedResult.Value.CompletedAt);
        Assert.InRange(completedResult.Value.CompletedAt!.Value, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

        // Act: toggle again (completed -> active)
        var reopenedResult = await _sut.ToggleTaskStatusAsync(7, CancellationToken.None);

        // Assert
        Assert.True(reopenedResult.IsSuccess);
        Assert.False(reopenedResult.Value!.IsCompleted);
        Assert.Null(reopenedResult.Value.CompletedAt);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ToggleStatus_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.ToggleTaskStatusAsync(999, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceResultErrorType.NotFound, result.ErrorType);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteTaskAsync

    [Fact]
    public async Task DeleteTask_WhenTaskExists_RemovesTask()
    {
        // Arrange
        var task = new TaskItem { Id = 3, Title = "Water the plants", Priority = TaskPriority.Low };
        _repositoryMock
            .Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repositoryMock
            .Setup(r => r.DeleteAsync(task, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteTaskAsync(3, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.DeleteAsync(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.DeleteTaskAsync(999, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceResultErrorType.NotFound, result.ErrorType);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}
