using Toarnbeike.Dispatch.Notifications;

namespace Toarnbeike.Dispatch.Tests.NotificationTestInstances;

internal sealed record Feedback(string Value) : INotificationFeedback;