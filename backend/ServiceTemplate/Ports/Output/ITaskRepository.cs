namespace ServiceTemplate.Ports.Output;

public interface ITaskRepository
{
    Task SaveAsync(TaskItem task);
    Task DeleteAsync(Guid id);
    Task<TaskItem?> LoadByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> ListAsync(int skip = 0, int take = 10);
}
