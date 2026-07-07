using NSubstitute;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra.Tests;

public class CachedWidgetRepositoryTests
{
    private readonly IWidgetRepository _repository;
    private readonly CachedWidgetRepository _cachedRepository;
    private readonly Widget _testWidget;

    public CachedWidgetRepositoryTests()
    {
        _repository = Substitute.For<IWidgetRepository>();
        _cachedRepository = new CachedWidgetRepository(_repository);
        _testWidget = new Widget(Guid.NewGuid(), "test-widget", DateTime.UtcNow, DateTime.UtcNow);
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldReturnFromCache_WhenItemIsCached()
    {
        // Arrange
        _repository.LoadByIdAsync(_testWidget.Id).Returns(_testWidget);
        await _cachedRepository.LoadByIdAsync(_testWidget.Id); // First call to cache the item

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Equal(_testWidget, result);
        await _repository.Received(1).LoadByIdAsync(_testWidget.Id); // Should only be called once
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldReturnFromRepository_WhenItemIsNotCached()
    {
        // Arrange
        _repository.LoadByIdAsync(_testWidget.Id).Returns(_testWidget);

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Equal(_testWidget, result);
        await _repository.Received(1).LoadByIdAsync(_testWidget.Id);
    }

    [Fact]
    public async Task LoadByIdAsync_ShouldNotCache_WhenRepositoryReturnsNull()
    {
        // Arrange
        _repository.LoadByIdAsync(_testWidget.Id).Returns((Widget?)null);

        // Act
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).LoadByIdAsync(_testWidget.Id);
    }

    [Fact]
    public async Task SaveAsync_ShouldPopulateCache_WhenSavingNewItem()
    {
        // Act
        await _cachedRepository.SaveAsync(_testWidget);
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Equal(_testWidget, result);
        await _repository.Received(0).LoadByIdAsync(_testWidget.Id); // Should not call repository
    }

    [Fact]
    public async Task UpdateAsync_ShouldRefreshCache_WhenItemWasAlreadyCached()
    {
        // Arrange
        await _cachedRepository.SaveAsync(_testWidget);
        var renamed = _testWidget with { Name = "renamed-widget" };

        // Act
        await _cachedRepository.UpdateAsync(renamed);
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Equal("renamed-widget", result!.Name);
        await _repository.Received(0).LoadByIdAsync(_testWidget.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldEvictFromCache()
    {
        // Arrange
        await _cachedRepository.SaveAsync(_testWidget);
        _repository.LoadByIdAsync(_testWidget.Id).Returns((Widget?)null);

        // Act
        await _cachedRepository.DeleteAsync(_testWidget.Id);
        var result = await _cachedRepository.LoadByIdAsync(_testWidget.Id);

        // Assert
        Assert.Null(result);
        await _repository.Received(1).LoadByIdAsync(_testWidget.Id); // Falls through to repository after eviction
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldDelegateToRepository()
    {
        // Arrange
        _repository.ExistsByNameAsync(_testWidget.Name).Returns(true);

        // Act
        var result = await _cachedRepository.ExistsByNameAsync(_testWidget.Name);

        // Assert
        Assert.True(result);
        await _repository.Received(1).ExistsByNameAsync(_testWidget.Name);
    }
}
