using Microsoft.Extensions.Logging.Abstractions;
using ServiceTemplate.Application;
using ServiceTemplate.Ports.Output;
using ServiceTemplate.Tests.Fakes;

namespace ServiceTemplate.Tests;

public class WidgetManagerTests
{
    private static readonly Guid GeneratedId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly IIdGenerator idGenerator;
    private readonly IWidgetRepository widgetRepository;
    private readonly SpyNotifier notifier;
    private readonly IClock clock;

    private readonly WidgetManager widgetManager;

    public WidgetManagerTests()
    {
        idGenerator = new StaticIdGenerator(GeneratedId);
        widgetRepository = new InMemoryWidgetRepository();
        notifier = new SpyNotifier();
        clock = new StaticClock();

        widgetManager = new WidgetManager(NullLogger<WidgetManager>.Instance, widgetRepository, idGenerator, notifier, clock);
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveWidgetAndNotify_WhenNameIsAvailable()
    {
        // Act
        var result = await widgetManager.CreateAsync("widget-one");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(GeneratedId.ToString(), result.Value.Id);
        Assert.Equal("widget-one", result.Value.Name);
        Assert.Equal(clock.UtcNow(), result.Value.CreatedAt);
        Assert.Equal(clock.UtcNow(), result.Value.UpdatedAt);

        var stored = await widgetRepository.LoadByIdAsync(GeneratedId);
        Assert.NotNull(stored);
        Assert.Equal("widget-one", stored.Name);

        Assert.Single(notifier.NotifiedWidgets);
        Assert.Equal(GeneratedId, notifier.NotifiedWidgets[0].Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNameAlreadyExists()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(Guid.NewGuid(), "taken-name", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.CreateAsync("taken-name");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("A widget named 'taken-name' already exists.", result.Error);
        Assert.Empty(notifier.NotifiedWidgets);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnWidget_WhenIdExists()
    {
        // Arrange
        var widget = new Widget(GeneratedId, "widget-one", clock.UtcNow(), clock.UtcNow());
        await widgetRepository.SaveAsync(widget);

        // Act
        var result = await widgetManager.GetAsync(GeneratedId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("widget-one", result.Value.Name);
    }

    [Fact]
    public async Task GetAsync_ShouldFail_WhenIdDoesNotExist()
    {
        // Act
        var result = await widgetManager.GetAsync(GeneratedId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The widget {GeneratedId} does not exist.", result.Error);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnWidgets_WhenWidgetsExist()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(Guid.NewGuid(), "widget-1", clock.UtcNow(), clock.UtcNow()));
        await widgetRepository.SaveAsync(new Widget(Guid.NewGuid(), "widget-2", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.ListAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task ListAsync_ShouldRespectPagination_WhenWidgetsExist()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            await widgetRepository.SaveAsync(new Widget(Guid.NewGuid(), $"widget-{i}", clock.UtcNow(), clock.UtcNow()));
        }

        // Act
        var result = await widgetManager.ListAsync(skip: 2, take: 2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldRenameWidget_WhenNewNameIsAvailable()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(GeneratedId, "old-name", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.UpdateAsync(GeneratedId, "new-name");

        // Assert
        Assert.True(result.IsSuccess);
        var stored = await widgetRepository.LoadByIdAsync(GeneratedId);
        Assert.Equal("new-name", stored!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenIdDoesNotExist()
    {
        // Act
        var result = await widgetManager.UpdateAsync(GeneratedId, "new-name");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The widget {GeneratedId} does not exist.", result.Error);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenNewNameIsTakenByAnotherWidget()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(GeneratedId, "widget-a", clock.UtcNow(), clock.UtcNow()));
        await widgetRepository.SaveAsync(new Widget(Guid.NewGuid(), "widget-b", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.UpdateAsync(GeneratedId, "widget-b");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("A widget named 'widget-b' already exists.", result.Error);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSucceed_WhenRenamingToItsOwnCurrentName()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(GeneratedId, "widget-a", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.UpdateAsync(GeneratedId, "widget-a");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveWidget_WhenIdExists()
    {
        // Arrange
        await widgetRepository.SaveAsync(new Widget(GeneratedId, "widget-a", clock.UtcNow(), clock.UtcNow()));

        // Act
        var result = await widgetManager.DeleteAsync(GeneratedId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(await widgetRepository.LoadByIdAsync(GeneratedId));
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenIdDoesNotExist()
    {
        // Act
        var result = await widgetManager.DeleteAsync(GeneratedId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"The widget {GeneratedId} does not exist.", result.Error);
    }
}
