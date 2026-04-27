using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Implementations;

internal sealed class RequestExecutor<TRequest, TResult>(
    IRequestHandler<TRequest, TResult> handler,
    IEnumerable<IPipelineBehavior<TRequest, TResult>> behaviors) : IRequestExecutor<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : notnull
{
    public async Task<Result<TResult>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
    {
        RequestHandlerDelegate<TResult> handlerDelegate = () => handler.HandleAsync(request, cancellationToken);

        var pipeline = behaviors
            .Reverse()
            .Aggregate(handlerDelegate, (next, behavior) =>
                () => behavior.HandleAsync(request, next, cancellationToken));

        return await pipeline();
    }
}