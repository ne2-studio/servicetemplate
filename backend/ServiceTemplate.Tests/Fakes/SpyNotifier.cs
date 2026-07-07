using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Tests.Fakes;

public class SpyNotifier : INotifier
{
    public List<Widget> NotifiedWidgets { get; } = new();

    public Task NotifyWidgetCreatedAsync(Widget widget)
    {
        NotifiedWidgets.Add(widget);
        return Task.CompletedTask;
    }
}
