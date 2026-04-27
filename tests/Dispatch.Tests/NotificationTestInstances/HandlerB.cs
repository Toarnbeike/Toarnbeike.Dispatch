using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class HandlerB(List<string> calls) : INotificationHandler<TestNotification>
{
    public Task<Result> Handle(TestNotification notification, INotificationFeedbackSink sink, CancellationToken ct)
    {
        calls.Add("B");
        sink.Report(new Feedback("B"));
        return Result.SuccessTask();
    }
}