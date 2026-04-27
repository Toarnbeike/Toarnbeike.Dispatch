using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Requests;

internal interface IRequestExecutor<in TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : notnull
{
    Task<Result<TResult>> ExecuteAsync(
        TRequest request,
        CancellationToken ct);
}