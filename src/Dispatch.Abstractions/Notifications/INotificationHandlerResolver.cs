namespace Toarnbeike.Dispatch.Notifications;

public interface INotificationHandlerResolver
{
    IReadOnlyList<INotificationHandler<TNotification>> ResolveNotificationHandlers<TNotification>()
        where TNotification : INotification;
}