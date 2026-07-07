using System.Collections.Concurrent;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra;

/// <summary>
/// Caching decorator over ITaskRepository, composed as a Singleton in ServiceRegistration
/// so the cache survives across requests (the deliberate exception to the default Scoped lifetime).
/// </summary>
public class CachedTaskRepository(ITaskRepository repository) : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, TaskItem> _cache = new();

    public async Task SaveAsync(TaskItem task)
    {
        await repository.SaveAsync(task);
        _cache[task.Id] = task;
    }

    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);
        _cache.TryRemove(id, out _);
    }

    public async Task<TaskItem?> LoadByIdAsync(Guid id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            return cached;
        }

        var task = await repository.LoadByIdAsync(id);
        if (task != null)
        {
            _cache[id] = task;
        }

        return task;
    }

    public Task<IEnumerable<TaskItem>> ListAsync(int skip = 0, int take = 10)
    {
        return repository.ListAsync(skip, take);
    }
}
