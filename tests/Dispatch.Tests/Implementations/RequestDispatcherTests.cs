using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Toarnbeike.Dispatch.Implementations;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;
using Toarnbeike.Results.TestExtensions;

namespace Toarnbeike.Dispatch.Tests.Implementations;

/// <remarks>
/// TestQuery must be public in order to make a Substitute.For{IRequestExecutor{TestQuery, int{>>}} in the tests,
/// as NSubstitute needs to create a proxy for the request type to verify that the correct request is passed to the executor.
/// </remarks>
public sealed record TestQuery : IQuery<int>;

public class RequestDispatcherTests
{
    private readonly IServiceScopeFactory _scopeFactory = Substitute.For<IServiceScopeFactory>();
    private readonly IServiceProvider _provider = Substitute.For<IServiceProvider>();
    private readonly RequestDispatcher _dispatcher;
    
    public RequestDispatcherTests()
    {
        var scope = Substitute.For<IServiceScope, IAsyncDisposable>();
        scope.ServiceProvider.Returns(_provider);
        _scopeFactory.CreateAsyncScope().Returns(scope);

        _dispatcher = new RequestDispatcher(_scopeFactory);
    }

    [Test]
    public async Task Dispatch_Should_CallExecutor_And_ReturnResult()
    {
        var request = new TestQuery();
        var expected = Result.Success(42);

        var executor = Substitute.For<IRequestExecutor<TestQuery, int>>();
        executor.ExecuteAsync(request, Arg.Any<CancellationToken>())
            .Returns(expected);

        _provider.GetService(typeof(IRequestExecutor<TestQuery, int>))
            .Returns(executor);

        var result = await _dispatcher.Dispatch(request);

        result.ShouldBeSuccess().ShouldBe(42);

        await executor.Received(1)
            .ExecuteAsync(request, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Dispatch_Should_ReturnMissingHandlerFailure_WhenNoExecutor()
    {
        var request = Substitute.For<IQuery<int>>();

        _provider.GetService(typeof(IRequestExecutor<IQuery<int>, int>))
            .Returns(null);

        var result = await _dispatcher.Dispatch(request);

        result.ShouldBeFailure().Message.ShouldContain("No handler found");
    }

    [Test]
    public async Task Dispatch_Should_Create_Scope()
    {
        var request = Substitute.For<IQuery<int>>();

        _provider.GetService(Arg.Any<Type>()).Returns(Substitute.For<IRequestExecutor<IQuery<int>, int>>());

        await _dispatcher.Dispatch(request);

        _scopeFactory.Received(1).CreateAsyncScope();
    }
}
