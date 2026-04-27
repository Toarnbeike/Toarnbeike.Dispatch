using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class TestHandler : IRequestHandler<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(TestQuery request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(42));
}