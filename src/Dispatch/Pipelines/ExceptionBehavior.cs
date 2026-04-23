using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;

namespace Toarnbeike.Dispatch.Pipelines;

/// <summary>
/// Exception handling behavior for the request pipeline.
/// Catches any unhandled exceptions that occur during the processing of a request and returns an <see cref="ExceptionFailure"/> with the exception details.
/// This ensures that exceptions are properly captured and returned as part of the result, allowing for better error handling and debugging.
/// </summary>
/// <typeparam name="TRequest">The type of request this behavior handles.</typeparam>
/// <typeparam name="TResult">The type of result produced by the request.</typeparam>
public class ExceptionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : notnull
{
    public async Task<Result<TResult>> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        // Option to catch specific exceptions can be added here.
        catch (Exception ex)
        {
            return new ExceptionFailure(ex);
        }
    }
}