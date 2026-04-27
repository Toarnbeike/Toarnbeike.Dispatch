using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

public interface INotificationFeedbackSink
{
    void Report(INotificationFeedback feedback);
    void ReportFailure(Failure failure);
    void ReportError(Exception exception);
}