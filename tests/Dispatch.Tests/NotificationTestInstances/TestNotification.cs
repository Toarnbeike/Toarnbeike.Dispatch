using Toarnbeike.Dispatch.Notifications;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed class TestNotification : INotification
{
    public Guid Id => Guid.CreateVersion7();
    public DateTimeOffset OccuredAt => DateTimeOffset.UtcNow;
}