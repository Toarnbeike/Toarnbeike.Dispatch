using System.ComponentModel;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Abstractions;

/// <summary>
/// Base interface for all requests handlers.
/// Do not use externally, use <c>ICommandHandler{TCommand}</c> or <c>IQueryHandler{TQuery,TResult}</c> instead.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handler implementation for the request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the request handling.</returns>
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}