using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

public interface INotificationPublishStrategy
{
    Task<Result> Publish<TNotification>(TNotification notification, IReadOnlyList<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken)
        where TNotification : INotification;
}