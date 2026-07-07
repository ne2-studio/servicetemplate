using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using ServiceTemplate.Ports.Input;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Application;

public class WidgetManager(
    ILogger<WidgetManager> logger,
    IWidgetRepository repository,
    IIdGenerator idGenerator,
    INotifier notifier,
    IClock clock) : IWidgetManager
{
    public async Task<Result<WidgetDto>> CreateAsync(string name)
    {
        logger.LogInformation("CreateAsync - Creating widget {Name}", name);

        if (await repository.ExistsByNameAsync(name))
        {
            logger.LogInformation("CreateAsync - Tried to create a widget with an existing name {Name}", name);
            return Result.Failure<WidgetDto>($"A widget named '{name}' already exists.");
        }

        var now = clock.UtcNow();
        var widget = new Widget(idGenerator.NewId(), name, now, now);

        try
        {
            await repository.SaveAsync(widget);
        }
        catch (InvalidOperationException)
        {
            // DB-level unique constraint caught a race the ExistsByNameAsync check above missed.
            logger.LogInformation("CreateAsync - Unique constraint rejected widget name {Name}", name);
            return Result.Failure<WidgetDto>($"A widget named '{name}' already exists.");
        }

        await notifier.NotifyWidgetCreatedAsync(widget);

        logger.LogInformation("CreateAsync - Widget {Id} created with name {Name}", widget.Id, name);
        return ToDto(widget);
    }

    public async Task<Result<WidgetDto>> GetAsync(Guid id)
    {
        logger.LogInformation("GetAsync - Fetching widget {Id}", id);
        var widget = await repository.LoadByIdAsync(id);

        if (widget == null)
        {
            logger.LogWarning("GetAsync - Widget {Id} does not exist", id);
            return Result.Failure<WidgetDto>($"The widget {id} does not exist.");
        }

        return ToDto(widget);
    }

    public async Task<Result<IEnumerable<WidgetDto>>> ListAsync(int skip = 0, int take = 10)
    {
        logger.LogInformation("ListAsync - Fetching widgets with skip {Skip} and take {Take}", skip, take);

        try
        {
            var widgets = await repository.ListAsync(skip, take);
            var dtos = widgets.Select(ToDto);
            logger.LogInformation("ListAsync - Successfully fetched {Count} widgets", widgets.Count());
            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ListAsync - Error occurred while fetching widgets");
            return Result.Failure<IEnumerable<WidgetDto>>("An error occurred while fetching the list of widgets.");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, string name)
    {
        logger.LogInformation("UpdateAsync - Updating widget {Id}", id);
        var widget = await repository.LoadByIdAsync(id);

        if (widget == null)
        {
            logger.LogWarning("UpdateAsync - Widget {Id} does not exist", id);
            return Result.Failure($"The widget {id} does not exist.");
        }

        if (!string.Equals(widget.Name, name, StringComparison.Ordinal) && await repository.ExistsByNameAsync(name))
        {
            logger.LogInformation("UpdateAsync - Tried to rename widget {Id} to an existing name {Name}", id, name);
            return Result.Failure($"A widget named '{name}' already exists.");
        }

        var updated = widget with { Name = name, UpdatedAt = clock.UtcNow() };
        await repository.UpdateAsync(updated);

        logger.LogInformation("UpdateAsync - Widget {Id} updated", id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        logger.LogInformation("DeleteAsync - Deleting widget {Id}", id);
        var widget = await repository.LoadByIdAsync(id);

        if (widget == null)
        {
            logger.LogWarning("DeleteAsync - Widget {Id} does not exist", id);
            return Result.Failure($"The widget {id} does not exist.");
        }

        await repository.DeleteAsync(id);

        logger.LogInformation("DeleteAsync - Widget {Id} deleted", id);
        return Result.Success();
    }

    private static WidgetDto ToDto(Widget widget) =>
        new(widget.Id.ToString(), widget.Name, widget.CreatedAt, widget.UpdatedAt);
}
