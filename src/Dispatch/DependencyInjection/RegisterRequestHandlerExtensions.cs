using Microsoft.Extensions.DependencyInjection;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.DependencyInjection;

public static class RegisterRequestHandlerExtensions
{
    /// <param name="services">The service collection to register the handler with.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a request handler for a command.
        /// </summary>
        /// <typeparam name="TCommand">The type of the request the handler will process.</typeparam>
        /// <typeparam name="TCommandHandler">The type of the handler to register.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection RegisterCommandHandler<TCommand, TCommandHandler>()
            where TCommand : ICommand
            where TCommandHandler : class, IRequestHandler<TCommand, CommandResponse>
        {
            return services.RegisterHandler<TCommand, CommandResponse, TCommandHandler>();
        }

        /// <summary>
        /// Registers a request handler for a command.
        /// </summary>
        /// <typeparam name="TCreateCommand">The type of the request the handler will process.</typeparam>
        /// <typeparam name="TKey">The type of the key associated with the create command. </typeparam>
        /// <typeparam name="TCommandHandler">The type of the handler to register.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection RegisterCreateCommandHandler<TCreateCommand, TKey, TCommandHandler>()
            where TCreateCommand : ICreateCommand<TKey>
            where TCommandHandler : class, IRequestHandler<TCreateCommand, CreateCommandResponse<TKey>>
            where TKey : notnull
        {
            return services.RegisterHandler<TCreateCommand, CreateCommandResponse<TKey>, TCommandHandler>();
        }

        /// <summary>
        /// Registers a request handler for a command.
        /// </summary>
        /// <typeparam name="TQuery">The type of the request the handler will process.</typeparam>
        /// <typeparam name="TResponse">The type of the key associated with the query. </typeparam>
        /// <typeparam name="TQueryHandler">The type of the handler to register.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection RegisterQueryHandler<TQuery, TResponse, TQueryHandler>()
            where TQuery : IQuery<TResponse>
            where TQueryHandler : class, IRequestHandler<TQuery, TResponse>
            where TResponse : notnull
        {
            return services.RegisterHandler<TQuery, TResponse, TQueryHandler>();
        }

        public IServiceCollection RegisterHandler<TRequest, TResponse, THandler>()
            where TRequest : IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
            where TResponse : notnull
        {
            services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
            return services;
        }
    }
}
