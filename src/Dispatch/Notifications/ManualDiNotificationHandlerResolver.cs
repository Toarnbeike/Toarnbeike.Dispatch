using Microsoft.Extensions.DependencyInjection;

namespace Toarnbeike.Dispatch.Notifications;

internal sealed class ManualDiNotificationHandlerResolver(IServiceProvider serviceProvider) : INotificationHandlerResolver
{
    public IReadOnlyList<INotificationHandler<TNotification>> ResolveNotificationHandlers<TNotification>()
        where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();
        return handlers.OrderBy(h => (h as IOrderedNotificationHandler)?.Order ?? 0)
            .ToArray();
    }
}