using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class FailingHandler : INotificationHandler<TestNotification>
{
    public Task<Result> Handle(TestNotification notification, INotificationFeedbackSink sink, CancellationToken ct)
        => Task.FromResult(Result.Failure(NotificationDispatchingTests.TestFailure));
}