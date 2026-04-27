using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class HandlerC(List<string> calls) : INotificationHandler<TestNotification>, IOrderedNotificationHandler
{
    public Task<Result> Handle(TestNotification notification, INotificationFeedbackSink sink, CancellationToken cancellationToken)
    {
        calls.Add("C");
        sink.Report(new Feedback("C"));
        return Result.SuccessTask();
    }

    public int Order => 1;
}