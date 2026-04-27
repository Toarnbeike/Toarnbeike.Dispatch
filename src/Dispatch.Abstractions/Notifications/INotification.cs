namespace Toarnbeike.Dispatch.Notifications;

/// <summary>
/// A notification is a request that can be handled by zero, one, or multiple handlers.
/// It can either be handled in a wait for result fashion, or as a fire and forget task.
/// </summary>
public interface INotification
{
    /// <summary>
    /// Unique identifier for this notification.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Metadata for the notification.
    /// </summary>
    DateTimeOffset OccuredAt { get; }
}