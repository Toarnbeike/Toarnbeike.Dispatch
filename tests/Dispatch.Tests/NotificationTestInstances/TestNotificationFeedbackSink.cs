using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class TestNotificationFeedbackSink : INotificationFeedbackSink
{
    public List<string> ReceivedDictionary { get; } = new();

    public void Report(INotificationFeedback feedback)
    {
        ReceivedDictionary.Add($"Feedback: {feedback}");
    }

    public void ReportFailure(Failure failure)
    {
        ReceivedDictionary.Add($"Failure: {failure.Message}");
    }

    public void ReportError(Exception exception)
    {
        ReceivedDictionary.Add($"Exception: {exception.Message}");
    }
}