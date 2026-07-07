namespace ServiceTemplate.Ports.Output;

public interface IWidgetRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task SaveAsync(Widget widget);
    Task UpdateAsync(Widget widget);
    Task DeleteAsync(Guid id);
    Task<Widget?> LoadByIdAsync(Guid id);
    Task<IEnumerable<Widget>> ListAsync(int skip = 0, int take = 10);
}
