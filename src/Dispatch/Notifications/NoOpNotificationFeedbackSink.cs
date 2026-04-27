using Microsoft.Extensions.Logging;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

internal sealed class NoOpNotificationFeedbackSink(ILogger<NoOpNotificationFeedbackSink> logger) : INotificationFeedbackSink
{
    public void Report(INotificationFeedback feedback)
    {
        logger.LogInformation("Received feedback: {Feedback}", feedback);
    }

    public void ReportFailure(Failure failure)
    {
        logger.LogWarning("Received Failure: {Message}", failure.Message);
    }

    public void ReportError(Exception exception)
    {
        logger.LogError(exception, "Received error: {Message}", exception.Message);
    }
}