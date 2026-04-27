using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;
using Toarnbeike.Results.Extensions;

namespace Toarnbeike.Dispatch.Pipelines;

/// <summary>
/// Configured options for the logging behavior.
/// </summary>
public sealed class LoggingBehaviorOptions
{
    /// <summary>
    /// A threshold which indicates the maximum duration of the pipeline. If the duration exceeds this threshold a warning if logged.
    /// </summary>
    public TimeSpan WarningThreshold { get; init; } = TimeSpan.FromSeconds(1);
}

/// <summary>
/// Behavior that logs the execution of a request, including the validation, handling, and unit of work results, as well as the total execution time.
/// If the execution time exceeds a specified threshold, a warning is logged.
/// </summary>
/// <typeparam name="TRequest">The request to apply the behavior to.</typeparam>
/// <typeparam name="TResult">The return type of the request.</typeparam>
/// <param name="logger">A logger to write the messages to.</param>
/// <param name="options">Configured options for the logging behavior.</param>
public sealed class LoggingBehavior<TRequest, TResult>(
    ILogger<LoggingBehavior<TRequest, TResult>> logger,
    IOptions<LoggingBehaviorOptions> options,
    TimeProvider timeProvider)
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : notnull
{
    /// <inheritdoc/>
    public async Task<Result<TResult>> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var start = timeProvider.GetTimestamp();
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Start handling request {RequestType}", requestName);

        var result = await next()
            .Tap(result => logger.LogInformation("Finished handling request {RequestType} (Result: '{Result}')", requestName, result))
            .TapFailure(failure => logger.LogWarning("Execution of request {RequestType} failed (Failure: '{Failure}')", requestName, failure.Message));

        var elapsed = timeProvider.GetElapsedTime(start).TotalMilliseconds;
        var threshold = options.Value.WarningThreshold.TotalMilliseconds;

        if (elapsed > threshold)
        {
            logger.LogWarning("Request {RequestType} took {Duration} ms, which exceeds the configured threshold of {Threshold} ms.", requestName, elapsed, threshold);
        }

        return result;
    }
}