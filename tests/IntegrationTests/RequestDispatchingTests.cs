using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.DependencyInjection;
using Toarnbeike.Dispatch.Failures;
using Toarnbeike.Dispatch.Pipelines;
using Toarnbeike.Dispatch.Tests.RequestTestInstances;
using Toarnbeike.Results.Failures;
using Toarnbeike.Results.TestExtensions;
using Toarnbeike.TestLibrary.Logging;

namespace Toarnbeike.Dispatch.Tests;

public class RequestDispatchingTests
{
    [Test]
    public async Task DispatchingRequest_EndToEnd()
    {
        var services = BuildServiceProvider(s =>
        {
            s.RegisterQueryHandler<TestQuery, int, TestHandler>();
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.ShouldBeSuccess().ShouldBe(42);
    }

    [Test]
    public async Task DispatchRequest_Should_UseSame_ScopedInstance_WithinRequest()
    {
        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<TestScoped>();

            s.AddScoped<IRequestHandler<TestQuery, int>>(sp =>
            {
                var scoped = sp.GetRequiredService<TestScoped>();
                return new TestHandlerScoped(scoped);
            });
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.ShouldBeSuccess().ShouldBe(1);
    }

    [Test]
    public async Task Dispatch_Should_RunPipelineBehavior()
    {
        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<IRequestHandler<TestQuery, int>, TestHandler>();
            s.AddScoped<IPipelineBehavior<TestQuery, int>, TestBehavior>();
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.ShouldBeSuccess().ShouldBe(43);
    }

    [Test]
    public async Task Dispatch_Should_ShortCircuit_When_Behavior_Fails()
    {
        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<IRequestHandler<TestQuery, int>, TestHandler>();
            s.AddScoped<IPipelineBehavior<TestQuery, int>, FailingBehavior>();
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.IsFailure.ShouldBeTrue();
    }

    [Test]
    public async Task Dispatch_Should_RespectBehaviorOrder()
    {
        var calls = new List<string>();

        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<IRequestHandler<TestQuery, int>>(_ => new TestHandlerOrdered(calls));
            s.AddScoped<IPipelineBehavior<TestQuery, int>>(_ => new OrderBehavior(1, calls));
            s.AddScoped<IPipelineBehavior<TestQuery, int>>(_ => new OrderBehavior(2, calls));
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        await dispatcher.Dispatch(new TestQuery());

        calls.ShouldBe(new[]
        {
            "Before 1",
            "Before 2",
            "Handler",
            "After 2",
            "After 1"
        });
    }

    [Test]
    public async Task Dispatch_Should_ReturnFailure_WhenHandlerNotRegistered()
    {
        var services = BuildServiceProvider();

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.ShouldBeFailureOfType<MissingHandlerFailure<TestQuery>>();
    }

    [Test]
    public async Task Dispatch_Should_LogFromPipeline()
    {
        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<IRequestHandler<TestQuery, int>, TestHandler>();
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        _ = await dispatcher.Dispatch(new TestQuery());

        var loggerProvider = services.GetTestLoggerProvider();

        loggerProvider.Assert()
            .WithCategory("Toarnbeike.Dispatch.Pipelines.LoggingBehavior")
            .WithLevel(LogLevel.Information)
            .WithMessageContaining("Start handling request TestQuery")
            .Single();
    }

    [Test]
    public async Task Dispatch_Should_HandleExceptionsToFailures()
    {
        var services = BuildServiceProvider(s =>
        {
            s.AddScoped<IRequestHandler<TestQuery, int>>(_ => new TestHandlerWithException());
        });

        var dispatcher = services.GetRequiredService<IRequestDispatcher>();

        var result = await dispatcher.Dispatch(new TestQuery());

        result.ShouldBeFailureOfType<ExceptionFailure>();
    }

    private static ServiceProvider BuildServiceProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddTestLoggerProvider();
        services.AddDispatching();

        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }
}