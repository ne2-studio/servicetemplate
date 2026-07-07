using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Tests.Fakes;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly Dictionary<Guid, TaskItem> storage = new();

    public Task SaveAsync(TaskItem task)
    {
        storage[task.Id] = task;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        storage.Remove(id);
        return Task.CompletedTask;
    }

    public Task<TaskItem?> LoadByIdAsync(Guid id)
    {
        return Task.FromResult(storage.GetValueOrDefault(id));
    }

    public Task<IEnumerable<TaskItem>> ListAsync(int skip = 0, int take = 10)
    {
        return Task.FromResult(storage.Values.Skip(skip).Take(take));
    }
}
