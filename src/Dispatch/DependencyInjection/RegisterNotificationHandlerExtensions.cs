using Microsoft.Extensions.DependencyInjection;
using Toarnbeike.Dispatch.Notifications;

namespace Toarnbeike.Dispatch.DependencyInjection;

public static class RegisterNotificationHandlerExtensions
{
    public static IServiceCollection RegisterNotificationHandler<TNotification, THandler>(
        this IServiceCollection services)
        where TNotification : INotification
        where THandler : class, INotificationHandler<TNotification>
    {
        services.AddTransient<INotificationHandler<TNotification>, THandler>();
        return services;
    }
}
