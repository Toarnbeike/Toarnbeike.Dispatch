namespace Toarnbeike.Dispatch.Notifications;

internal interface INotificationHandlerResolver
{
    IReadOnlyList<INotificationHandler<TNotification>> ResolveNotificationHandlers<TNotification>()
        where TNotification : INotification;
}