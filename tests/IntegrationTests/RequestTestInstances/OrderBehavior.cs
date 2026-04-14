using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class OrderBehavior(int order, List<string> calls) : IPipelineBehavior<TestQuery, int>
{
    public async Task<Result<int>> HandleAsync(
        TestQuery request,
        RequestHandlerDelegate<int> next,
        CancellationToken ct)
    {
        calls.Add($"Before {order}");
        var result = await next();
        calls.Add($"After {order}");
        return result;
    }
}