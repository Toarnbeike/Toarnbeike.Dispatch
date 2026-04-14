using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;
using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.Failures;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Dispatch.Responses;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Implementations;

internal sealed class RequestDispatcher(IServiceScopeFactory scopeFactory) : IRequestDispatcher
{
    private readonly ConcurrentDictionary<(Type Request, Type Result), Func<IServiceProvider, object, CancellationToken, Task<object>>> _cache = new();

    public Task<Result<TResult>> Dispatch<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResult>
    {
        return DispatchInternal<TRequest, TResult>(request, cancellationToken);
    }

    /// <summary>
    /// <inheritdoc />
    /// <para>If the command implements <see cref="MightRequireUserConfirmationCommand"/>, it will be dispatched as such to allow the handler to check if the command is confirmed or not and act accordingly.</para>
    /// </summary>
    public Task<Result<CommandResponse>> Dispatch(ICommand command, CancellationToken cancellationToken = default) =>
        DispatchInternal<ICommand, CommandResponse>(command, cancellationToken);

    /// <inheritdoc />
    public Task<Result<CreateCommandResponse<TKey>>> Dispatch<TKey>(ICreateCommand<TKey> command, CancellationToken cancellationToken = default) =>
        DispatchInternal<ICreateCommand<TKey>, CreateCommandResponse<TKey>>(command, cancellationToken);

    /// <inheritdoc />
    public Task<Result<TResult>> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
        DispatchInternal<IQuery<TResult>, TResult>(query, cancellationToken);

    private async Task<Result<TResult>> DispatchInternal<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var requestType = request.GetType();
        var resultType = typeof(TResult);

        var handler = _cache.GetOrAdd((requestType, resultType), static key =>
        {
            var (reqType, resType) = key;

            var method = typeof(RequestDispatcher)
                .GetMethod(nameof(InvokeExecutor), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(reqType, resType);

            return (sp, req, ct) =>
                (Task<object>)method.Invoke(null, [sp, req, ct])!;
        });

        var result = await handler(scope.ServiceProvider, request, cancellationToken);

        return (Result<TResult>)result;
    }

    private static async Task<object> InvokeExecutor<TRequest, TResult>(
        IServiceProvider sp,
        object request,
        CancellationToken ct)
        where TRequest : IRequest<TResult>
    {
        try
        {
            var executor = sp.GetService<IRequestExecutor<TRequest, TResult>>();
            return executor is not null
                ?  await executor.ExecuteAsync((TRequest)request, ct)
                : Result<TResult>.Failure(new MissingHandlerFailure<TRequest>((TRequest)request));
        }
        catch 
        {
            return Result<TResult>.Failure(new MissingHandlerFailure<TRequest>((TRequest)request));
        }
    }

    //private async Task<Result<TResult>> DispatchInternal<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
    //    where TRequest : IRequest<TResult>
    //{
    //    await using var scope = scopeFactory.CreateAsyncScope();

    //    var executor = scope.ServiceProvider.GetService<IRequestExecutor<TRequest, TResult>>();
    //    return executor != null
    //        ? await executor.ExecuteAsync(request, cancellationToken)
    //        : new MissingHandlerFailure<TRequest>(request);
    //}
}