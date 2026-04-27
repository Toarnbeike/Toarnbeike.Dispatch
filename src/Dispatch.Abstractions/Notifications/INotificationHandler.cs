using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Notifications;

public interface IOrderedNotificationHandler
{
    int Order { get; }
}

public interface INotificationHandler<in TNotification> 
    where TNotification : INotification
{
    Task<Result> Handle(TNotification notification, INotificationFeedbackSink sink, CancellationToken cancellationToken);
}