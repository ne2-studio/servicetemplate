namespace ServiceTemplate.Ports.Output;

/// <summary>
/// Notifies external systems about widget lifecycle events (e.g. a webhook or message queue).
/// Toggled between a real adapter and a no-op via the "Features:Notifications:Enabled" config flag,
/// composed in ServiceTemplate.Infra.ServiceRegistration.
/// </summary>
public interface INotifier
{
    Task NotifyWidgetCreatedAsync(Widget widget);
}
