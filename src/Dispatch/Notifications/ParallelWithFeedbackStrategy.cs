using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

internal sealed class ParallelWithFeedbackStrategy(INotificationFeedbackSink sink) : INotificationPublishStrategy
{
    public Task<Result> Publish<TNotification>(TNotification notification, IReadOnlyList<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken) where TNotification : INotification
    {
        foreach (var handler in handlers)
        {
            Task.Run(async () => { await Fire(notification, handler); }, CancellationToken.None);
        }
        return Result.SuccessTask();
    }

    private async Task Fire<TNotification>(TNotification notification, INotificationHandler<TNotification> handler)
        where TNotification : INotification
    {
        try
        {
            var result = await handler.Handle(notification, sink, CancellationToken.None);
            if (result.TryGetFailure(out var failure))
            {
                sink.ReportFailure(failure);
            }
        }
        catch (Exception ex)
        {
            sink.ReportError(ex);
        }
    }
}