using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

internal sealed class NotificationDispatcher(
    INotificationHandlerResolver resolver,
    SequentialStrategy sequentialStrategy,
    ParallelWithFeedbackStrategy parallelWithFeedbackStrategy) : INotificationDispatcher
{
    public Task<Result> Publish<TNotification>(TNotification notification, PublishingStrategy strategy, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = resolver.ResolveNotificationHandlers<TNotification>();
        return strategy switch
        {
            PublishingStrategy.Sequential => sequentialStrategy.Publish(notification, handlers, cancellationToken),
            PublishingStrategy.ParallelWithFeedback => parallelWithFeedbackStrategy.Publish(notification, handlers,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }
}