using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Tests.RequestTestInstances;

internal sealed class TestHandlerScoped(TestScoped scoped) : IRequestHandler<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(TestQuery request, CancellationToken ct)
    {
        scoped.Counter++;
        return Task.FromResult(Result.Success(scoped.Counter));
    }
}