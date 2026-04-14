using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toarnbeike.Dispatch.Implementations;
using Toarnbeike.Dispatch.Pipelines;

namespace Toarnbeike.Dispatch.DependencyInjection;

public static partial class RegisterDispatchingExtensions
{
    public static IServiceCollection AddDispatching(this IServiceCollection services, Action<LoggingBehaviorOptions>? configureOptions = null)
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

        RegisterSourceGeneratedHandlers(services);
        return services;
    }

    static partial void RegisterSourceGeneratedHandlers(IServiceCollection services);
}