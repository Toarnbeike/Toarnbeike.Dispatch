using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;
using Toarnbeike.Testing.Logging;

namespace Toarnbeike.Dispatch.Tests.Pipelines;

internal class LoggingPipelineBehaviorTests
{
    private readonly IQuery<int> _request = Substitute.For<IQuery<int>>();
    private readonly FakeTimeProvider _fakeTimeProvider = new();

    [Test]
    public async Task HandleAsync_Should_LogStart_AndFinish_OnSuccess()
    {
        var logger = new TestLogger<LoggingBehavior<IQuery<int>, int>>(nameof(LoggingBehavior<,>));
        var behavior = Create(logger);

        RequestHandlerDelegate<int> next = () =>
            Task.FromResult(Result.Success(42));

        await behavior.HandleAsync(_request, next);

        logger.Assert()
            .WithLevel(LogLevel.Information)
            .WithMessageContaining("Start handling request")
            .Single();

        logger.Assert()
            .WithLevel(LogLevel.Information)
            .WithMessageContaining("Finished handling request")
            .Single();
    }

    [Test]
    public async Task HandleAsync_Should_LogWarning_OnFailure()
    {
        var logger = new TestLogger<LoggingBehavior<IQuery<int>, int>>(nameof(LoggingBehavior<,>));
        var behavior = Create(logger);

        var failure = new ExceptionFailure(new Exception("boom"));

        RequestHandlerDelegate<int> next = () =>
            Task.FromResult(Result<int>.Failure(failure));

        await behavior.HandleAsync(_request, next);

        logger.Assert()
            .WithLevel(LogLevel.Warning)
            .WithMessageContaining("failed")
            .Single();
    }

    [Test]
    public async Task HandleAsync_Should_LogWarning_WhenThresholdExceeded()
    {
        var logger = new TestLogger<LoggingBehavior<IQuery<int>, int>>(nameof(LoggingBehavior<,>));
        var behavior = Create(logger, TimeSpan.FromMilliseconds(100), _fakeTimeProvider);

        RequestHandlerDelegate<int> next = () =>
        {
            _fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(200));
            return Task.FromResult(Result.Success(1));
        };

        await behavior.HandleAsync(_request, next);

        logger.Assert()
            .WithLevel(LogLevel.Warning)
            .WithMessageContaining("exceeds")
            .Single();
    }

    [Test]
    public async Task HandleAsync_Should_NotLogWarning_WhenThresholdIsNotExceeded()
    {
        var logger = new TestLogger<LoggingBehavior<IQuery<int>, int>>(nameof(LoggingBehavior<,>));
        var behavior = Create(logger, TimeSpan.FromMilliseconds(200), _fakeTimeProvider);

        RequestHandlerDelegate<int> next = () =>
        {
            _fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(100));
            return Task.FromResult(Result.Success(1));
        };

        await behavior.HandleAsync(_request, next);

        logger.Assert()
            .WithLevel(LogLevel.Warning)
            .WithMessageContaining("exceeds")
            .None();
    }

    private static LoggingBehavior<TRequest, TResult> Create<TRequest, TResult>(
        ILogger<LoggingBehavior<TRequest, TResult>> logger,
        TimeSpan? threshold = null,
        TimeProvider? timeProvider = null)
        where TRequest : IRequest<TResult>
        where TResult : notnull
    {
        var options = Options.Create(new LoggingBehaviorOptions
        {
            WarningThreshold = threshold ?? TimeSpan.FromSeconds(1)
        });

        return new LoggingBehavior<TRequest, TResult>(logger, options, timeProvider ?? TimeProvider.System);
    }
}
