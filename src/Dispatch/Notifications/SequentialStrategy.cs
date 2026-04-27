using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

internal sealed class SequentialStrategy(INotificationFeedbackSink sink) : INotificationPublishStrategy
{
    public async Task<Result> Publish<TNotification>(TNotification notification,
        IReadOnlyList<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        foreach (var handler in handlers)
        {
            var result = await handler.Handle(notification, sink, cancellationToken);

            if (result.TryGetFailure(out var failure))
                return failure;
        }

        return Result.Success();
    }
}