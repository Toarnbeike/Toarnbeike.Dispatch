using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class TestHandlerWithException : IRequestHandler<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(TestQuery request, CancellationToken ct)
    {
        throw new Exception("Handler exception");
    }
}