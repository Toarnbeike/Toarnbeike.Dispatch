using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;
using Toarnbeike.Results.TestExtensions;

namespace Toarnbeike.Dispatch.Tests.Pipelines;

public class ExceptionBehaviorTests
{
    [Test]
    public async Task HandleAsync_ReturnsResult_WhenNoException()
    {
        var request = Substitute.For<IQuery<int>>();
        var next = Substitute.For<RequestHandlerDelegate<int>>();

        next.Invoke().Returns(Result.Success(42));

        var behavior = new ExceptionBehavior<IQuery<int>, int>();

        var result = await behavior.HandleAsync(request, next, CancellationToken.None);

        result.ShouldBeSuccess().ShouldBe(42);
    }

    [Test]
    public async Task HandleAsync_HandlesException_AndReturnsFailure()
    {
        var request = Substitute.For<IQuery<int>>();
        var handler = Substitute.For<RequestHandlerDelegate<int>>();

        handler.Invoke()
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        var behavior = new ExceptionBehavior<IQuery<int>, int>();

        var result = await behavior.HandleAsync(request, handler, CancellationToken.None);

        var failure = result.ShouldBeFailureOfType<ExceptionFailure>();
        var actualException = failure.Exception.ShouldBeOfType<InvalidOperationException>();
        actualException.Message.ShouldBe("Test exception");
    }
}
