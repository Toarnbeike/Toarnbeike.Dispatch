using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch;

public interface INotificationDispatcher
{
    Task<Result> Publish<TNotification>(TNotification notification, PublishingStrategy strategy, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}