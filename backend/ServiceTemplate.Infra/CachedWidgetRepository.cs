using System.Collections.Concurrent;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra;

/// <summary>
/// Caching decorator over IWidgetRepository, composed as a Singleton in ServiceRegistration
/// so the cache survives across requests (the deliberate exception to the default Scoped lifetime).
/// </summary>
public class CachedWidgetRepository(IWidgetRepository repository) : IWidgetRepository
{
    private readonly ConcurrentDictionary<Guid, Widget> _cache = new();

    public Task<bool> ExistsByNameAsync(string name)
    {
        return repository.ExistsByNameAsync(name);
    }

    public async Task SaveAsync(Widget widget)
    {
        await repository.SaveAsync(widget);
        _cache[widget.Id] = widget;
    }

    public async Task UpdateAsync(Widget widget)
    {
        await repository.UpdateAsync(widget);
        _cache[widget.Id] = widget;
    }

    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);
        _cache.TryRemove(id, out _);
    }

    public async Task<Widget?> LoadByIdAsync(Guid id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            return cached;
        }

        var widget = await repository.LoadByIdAsync(id);
        if (widget != null)
        {
            _cache[id] = widget;
        }

        return widget;
    }

    public Task<IEnumerable<Widget>> ListAsync(int skip = 0, int take = 10)
    {
        return repository.ListAsync(skip, take);
    }
}
