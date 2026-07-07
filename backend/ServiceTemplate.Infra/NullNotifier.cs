using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra;

public class NullNotifier : INotifier
{
    public Task NotifyWidgetCreatedAsync(Widget widget)
    {
        return Task.CompletedTask;
    }
}
