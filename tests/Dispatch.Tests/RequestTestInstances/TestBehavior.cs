using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Results;
using Toarnbeike.Results.Extensions;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class TestBehavior : IPipelineBehavior<TestQuery, int>
{
    public async Task<Result<int>> HandleAsync(
        TestQuery request,
        RequestHandlerDelegate<int> next,
        CancellationToken ct)
    {
        return await next()
            .Map(value => value + 1);
    }
}