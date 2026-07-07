using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Tests.Fakes;

public class InMemoryWidgetRepository : IWidgetRepository
{
    private readonly Dictionary<Guid, Widget> storage = new();

    public Task<bool> ExistsByNameAsync(string name)
    {
        return Task.FromResult(storage.Values.Any(w => w.Name == name));
    }

    public Task SaveAsync(Widget widget)
    {
        if (!storage.TryAdd(widget.Id, widget))
        {
            throw new InvalidOperationException("Widget already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Widget widget)
    {
        if (!storage.ContainsKey(widget.Id))
            throw new InvalidOperationException("Widget not found.");
        storage[widget.Id] = widget;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        storage.Remove(id);
        return Task.CompletedTask;
    }

    public Task<Widget?> LoadByIdAsync(Guid id)
    {
        return Task.FromResult(storage.GetValueOrDefault(id));
    }

    public Task<IEnumerable<Widget>> ListAsync(int skip = 0, int take = 10)
    {
        return Task.FromResult(storage.Values.Skip(skip).Take(take));
    }
}
