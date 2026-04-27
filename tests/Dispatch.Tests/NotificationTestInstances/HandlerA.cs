using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class HandlerA(List<string> calls) : INotificationHandler<TestNotification>
{
    public Task<Result> Handle(TestNotification notification, INotificationFeedbackSink sink, CancellationToken ct)
    {
        calls.Add("A");
        sink.Report(new Feedback("A"));
        return Result.SuccessTask();
    }
}