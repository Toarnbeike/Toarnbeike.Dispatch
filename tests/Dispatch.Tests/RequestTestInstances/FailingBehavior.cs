using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class FailingBehavior : IPipelineBehavior<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(
        TestQuery request,
        RequestHandlerDelegate<int> next,
        CancellationToken ct)
    {
        return Task.FromResult(Result<int>.Failure(
            new ExceptionFailure(new Exception("fail"))));
    }
}