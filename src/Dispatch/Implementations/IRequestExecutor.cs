using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Implementations;

public interface IRequestExecutor<in TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : notnull
{
    Task<Result<TResult>> ExecuteAsync(
        TRequest request,
        CancellationToken ct);
}