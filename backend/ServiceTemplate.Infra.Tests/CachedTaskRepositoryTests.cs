using NSubstitute;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra.Tests;

public class CachedTaskRepositoryTests
{
    private readonly ITaskRepository _repository;
    private readonly CachedTaskRepository _cachedRepository;
    private readonly TaskItem _testTask;

    public CachedTaskRepositoryTests()
    {
        _repository = Substitute.For<ITaskRepository>();
        _cachedRepository = new CachedTaskRepository(_repository);
        _testTask = new TaskItem(Guid.NewGuid(), "test-task", DateTime.UtcNow);
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldReturnFromCache_WhenItemIsCached()
    {
        // Arrange
        _repository.LoadByIdAsync(_testTask.Id).Returns(_testTask);
        await _cachedRepository.LoadByIdAsync(_testTask.Id); // First call to cache the item

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testTask.Id);

        // Assert
        Assert.Equal(_testTask, result);
        await _repository.Received(1).LoadByIdAsync(_testTask.Id); // Should only be called once
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldReturnFromRepository_WhenItemIsNotCached()
    {
        // Arrange
        _repository.LoadByIdAsync(_testTask.Id).Returns(_testTask);

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testTask.Id);

        // Assert
        Assert.Equal(_testTask, result);
        await _repository.Received(1).LoadByIdAsync(_testTask.Id);
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldNotCache_WhenRepositoryReturnsNull()
    {
        // Arrange
        _repository.LoadByIdAsync(_testTask.Id).Returns((TaskItem?)null);

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testTask.Id);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).LoadByIdAsync(_testTask.Id);
    }

    [Fact]
    public async Task SaveAsync_ShouldPopulateCache_WhenSavingNewItem()
    {
        // Act
        await _cachedRepository.SaveAsync(_testTask);
        var result = await _cachedRepository.LoadByIdAsync(_testTask.Id);

        // Assert
        Assert.Equal(_testTask, result);
        await _repository.Received(0).LoadByIdAsync(_testTask.Id); // Should not call repository
    }

    [Fact]
    public async Task DeleteAsync_ShouldEvictFromCache()
    {
        // Arrange
        await _cachedRepository.SaveAsync(_testTask);
        _repository.LoadByIdAsync(_testTask.Id).Returns((TaskItem?)null);

        // Act
        await _cachedRepository.DeleteAsync(_testTask.Id);
        var result = await _cachedRepository.LoadByIdAsync(_testTask.Id);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).LoadByIdAsync(_testTask.Id); // Falls through to repository after eviction
    }
}
