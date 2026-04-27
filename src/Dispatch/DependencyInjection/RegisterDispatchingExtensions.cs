using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Dispatch.Requests;

namespace Toarnbeike.Dispatch.DependencyInjection;

public static class RegisterDispatchingExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRequestDispatching(Action<LoggingBehaviorOptions>? configureOptions = null)
        {
            services.AddSingleton<IRequestDispatcher, RequestDispatcher>();
            services.AddScoped(typeof(IRequestExecutor<,>), typeof(RequestExecutor<,>));
            services.TryAddSingleton(TimeProvider.System);
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
        
            return services;
        }

        public IServiceCollection AddNotificationDispatching()
        {
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            services.AddScoped<INotificationHandlerResolver, ManualDiNotificationHandlerResolver>();
            services.TryAddSingleton<INotificationFeedbackSink, NoOpNotificationFeedbackSink>();
            services.AddSingleton<ParallelWithFeedbackStrategy>();
            services.AddSingleton<SequentialStrategy>();
            return services;
        }
    }
}