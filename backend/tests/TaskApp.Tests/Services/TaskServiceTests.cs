using Moq;
using TaskApp.Application.DTOs;
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
}
