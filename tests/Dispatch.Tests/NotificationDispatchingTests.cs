using Microsoft.Extensions.DependencyInjection;
using Toarnbeike.Dispatch.DependencyInjection;
using Toarnbeike.Dispatch.Notifications;
using Toarnbeike.Dispatch.Tests.NotificationTestInstances;
using Toarnbeike.Results.Failures;
using Toarnbeike.Results.TestExtensions;
using Toarnbeike.Testing.Logging;

namespace Toarnbeike.Dispatch.Tests;

public class NotificationDispatchingTests
{
    internal static readonly SimpleFailure TestFailure = new("Code", "Message");

    private static ServiceProvider CreateProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddTestLoggerProvider();
        services.AddNotificationDispatching();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    [Test]
    public async Task Sequential_Should_InvokeAllHandlers_InOrder()
    {
        var calls = new List<string>();

        var provider = CreateProvider(services =>
        {
            services.RegisterNotificationHandler<TestNotification, HandlerA>();
            services.RegisterNotificationHandler<TestNotification, HandlerB>();

            services.AddSingleton(calls);
        });

        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();

        var result = await dispatcher.Publish(new TestNotification(), PublishingStrategy.Sequential);

        result.ShouldBeSuccess();
        calls.ShouldBe(["A", "B"]);
    }

    [Test]
    public async Task Sequential_Should_StopOnException()
    {
        var calls = new List<string>();

        var provider = CreateProvider(services =>
        {
            services.RegisterNotificationHandler<TestNotification, FailingHandler>();
            services.RegisterNotificationHandler<TestNotification, HandlerB>();
            services.AddSingleton(calls);
        });

        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();

        var result = await dispatcher.Publish(new TestNotification(), PublishingStrategy.Sequential);

        result.ShouldBeFailure().ShouldBe(TestFailure);
        calls.ShouldBeEmpty(); // HandlerB should not have been called.
    }

    [Test]
    public async Task Sequential_Should_RespectOrder_WhenProvided()
    {
        var calls = new List<string>();

        var provider = CreateProvider(services =>
        {
            services.RegisterNotificationHandler<TestNotification, HandlerC>();
            services.RegisterNotificationHandler<TestNotification, HandlerB>();

            services.AddSingleton(calls);
        });

        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();

        var result = await dispatcher.Publish(new TestNotification(), PublishingStrategy.Sequential);

        result.ShouldBeSuccess();
        calls.ShouldBe(["B", "C"]); // HandlerC is registered first, but moved down because it has an order > 0
    }

    [Test]
    public async Task Parallel_Should_InvokeAllHandlers_AndReportFeedback()
    {
        var calls = new List<string>();
        var feedbackSink = new TestNotificationFeedbackSink();

        var provider = CreateProvider(services =>
        {
            services.RegisterNotificationHandler<TestNotification, HandlerA>();
            services.RegisterNotificationHandler<TestNotification, HandlerB>();

            services.AddSingleton<INotificationFeedbackSink>(feedbackSink);
            services.AddSingleton(calls);
        });

        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();

        var result = await dispatcher.Publish(new TestNotification(), PublishingStrategy.ParallelWithFeedback);

        result.ShouldBeSuccess();
        
        await Task.Delay(5);
        feedbackSink.ReceivedDictionary.Count.ShouldBe(2);
        feedbackSink.ReceivedDictionary.ShouldContain("Feedback: Feedback { Value = A }");
        feedbackSink.ReceivedDictionary.ShouldContain("Feedback: Feedback { Value = B }");
    }

    [Test]
    public async Task Parallel_Should_InvokeAllHandlers_AndReportFeedback_Duplicate()
    {
        var calls = new List<string>();
        var feedbackSink = new TestNotificationFeedbackSink();

        var provider = CreateProvider(services =>
        {
            services.RegisterNotificationHandler<TestNotification, FailingHandler>();
            services.RegisterNotificationHandler<TestNotification, HandlerB>();

            services.AddSingleton<INotificationFeedbackSink>(feedbackSink);
            services.AddSingleton(calls);
        });

        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();

        var result = await dispatcher.Publish(new TestNotification(), PublishingStrategy.ParallelWithFeedback);

        result.ShouldBeSuccess();

        await Task.Delay(5);
        feedbackSink.ReceivedDictionary.Count.ShouldBe(2);
        feedbackSink.ReceivedDictionary.ShouldContain("Failure: Message");
        feedbackSink.ReceivedDictionary.ShouldContain("Feedback: Feedback { Value = B }");
    }

    [Test]
    public async Task NoHandlers_Should_NotThrow()
    {
        var provider = CreateProvider();
        var dispatcher = provider.GetRequiredService<INotificationDispatcher>();
        await Should.NotThrowAsync(() => dispatcher.Publish(new TestNotification(), PublishingStrategy.Sequential));
        await Should.NotThrowAsync(() =>
            dispatcher.Publish(new TestNotification(), PublishingStrategy.ParallelWithFeedback));
    }
}