using NSubstitute;
using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;

namespace Toarnbeike.Dispatch.Tests.Requests;

public class RequestExecutorTests
{
    [Test]
    public async Task ExecuteAsync_Should_CallHandler_WhenNoBehaviors()
    {
        var request = Substitute.For<IQuery<int>>();
        var handler = Substitute.For<IRequestHandler<IQuery<int>, int>>();

        var expected = Result.Success(42);

        handler.HandleAsync(request, CancellationToken.None)
            .Returns(expected);

        var executor = Create(handler);

        var result = await executor.ExecuteAsync(request, CancellationToken.None);

        result.ShouldBe(expected);

        await handler.Received(1)
            .HandleAsync(request, CancellationToken.None);
    }

    [Test]
    public async Task ExecuteAsync_Should_InvokeBehavior_BeforeHandler()
    {
        var request = Substitute.For<IQuery<int>>();

        var handler = Substitute.For<IRequestHandler<IQuery<int>, int>>();
        handler.HandleAsync(request, CancellationToken.None)
            .Returns(Result.Success(1));

        var behavior = Substitute.For<IPipelineBehavior<IQuery<int>, int>>();

        behavior.HandleAsync(
                request,
                Arg.Any<RequestHandlerDelegate<int>>(),
                CancellationToken.None)
            .Returns(async callInfo =>
            {
                var next = callInfo.Arg<RequestHandlerDelegate<int>>();
                return await next();
            });

        var executor = Create(handler, behavior);

        await executor.ExecuteAsync(request, CancellationToken.None);

        await behavior.Received(1)
            .HandleAsync(request, Arg.Any<RequestHandlerDelegate<int>>(), CancellationToken.None);

        await handler.Received(1)
            .HandleAsync(request, CancellationToken.None);
    }

    [Test]
    public async Task ExecuteAsync_Should_RespectBehaviorOrder()
    {
        var request = Substitute.For<IQuery<int>>();
        var handler = Substitute.For<IRequestHandler<IQuery<int>, int>>();

        var calls = new List<string>();

        handler.HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                calls.Add("handler");
                return Result.Success(0);
            });

        var behavior1 = Substitute.For<IPipelineBehavior<IQuery<int>, int>>();
        behavior1.HandleAsync(request, Arg.Any<RequestHandlerDelegate<int>>(), Arg.Any<CancellationToken>())
            .Returns(async ci =>
            {
                calls.Add("b1-before");
                var result = await ci.Arg<RequestHandlerDelegate<int>>()();
                calls.Add("b1-after");
                return result;
            });

        var behavior2 = Substitute.For<IPipelineBehavior<IQuery<int>, int>>();
        behavior2.HandleAsync(request, Arg.Any<RequestHandlerDelegate<int>>(), Arg.Any<CancellationToken>())
            .Returns(async ci =>
            {
                calls.Add("b2-before");
                var result = await ci.Arg<RequestHandlerDelegate<int>>()();
                calls.Add("b2-after");
                return result;
            });

        var executor = Create(handler, behavior1, behavior2);

        await executor.ExecuteAsync(request, CancellationToken.None);

        calls.ShouldBe(new[]
        {
            "b1-before",
            "b2-before",
            "handler",
            "b2-after",
            "b1-after"
        });
    }

    [Test]
    public async Task ExecuteAsync_Should_ShortCircuit_When_BehaviorDoesNotCallNext()
    {
        var request = Substitute.For<IQuery<int>>();
        var handler = Substitute.For<IRequestHandler<IQuery<int>, int>>();

        var failure = Result<int>.Failure(new ExceptionFailure(new Exception("fail")));

        var behavior = Substitute.For<IPipelineBehavior<IQuery<int>, int>>();
        behavior.HandleAsync(
                request,
                Arg.Any<RequestHandlerDelegate<int>>(),
                CancellationToken.None)
            .Returns(failure); // doesn't call next, short-circuiting the pipeline

        var executor = Create(handler, behavior);

        var result = await executor.ExecuteAsync(request, CancellationToken.None);

        result.ShouldBe(failure);

        await handler.DidNotReceive()
            .HandleAsync(Arg.Any<IQuery<int>>(), Arg.Any<CancellationToken>());
    }

    private static RequestExecutor<TRequest, TResult> Create<TRequest, TResult>(
        IRequestHandler<TRequest, TResult> handler,
        params IPipelineBehavior<TRequest, TResult>[] behaviors)
        where TRequest : IRequest<TResult>
        where TResult : notnull
    {
        return new RequestExecutor<TRequest, TResult>(handler, behaviors);
    }
}
