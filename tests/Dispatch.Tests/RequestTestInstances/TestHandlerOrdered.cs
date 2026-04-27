using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class TestHandlerOrdered(List<string> calls) : IRequestHandler<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(TestQuery request, CancellationToken ct)
    {
        calls.Add("Handler");
        return Task.FromResult(Result.Success(42));
    }
}