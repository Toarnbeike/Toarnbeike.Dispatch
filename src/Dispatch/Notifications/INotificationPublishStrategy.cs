using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

internal interface INotificationPublishStrategy
{
    Task<Result> Publish<TNotification>(TNotification notification, IReadOnlyList<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken)
        where TNotification : INotification;
}