using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Pipelines;

public delegate Task<Result<TResult>> RequestHandlerDelegate<TResult>();

public interface IPipelineBehavior<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Processes the specified request asynchronously within the pipeline and returns the result produced by the operation.
    /// </summary>
    /// <remarks>This method is typically implemented by pipeline behaviors to add custom logic before or
    /// after the request is handled. It enables chaining of multiple behaviors and can be used for cross-cutting
    /// concerns such as logging, validation, or exception handling.</remarks>
    /// <param name="request">The request that starts the pipeline.</param>
    /// <param name="next">A delegate representing the next handler in the pipeline, which can be invoked to continue processing the request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the outcome of the request processing.</returns>
    Task<Result<TResult>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken = default);
}