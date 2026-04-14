using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Dispatch.Responses;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch;

public interface IRequestDispatcher
{
    Task<Result<TResult>> Dispatch<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResult>;

    /// <summary>
    /// Handle the given command and return the result of the command execution.
    /// The result can be a success or a failure, and it may contain additional information about the execution,
    /// such as an undo description or a toast message content.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command execution.</returns>
    Task<Result<CommandResponse>> Dispatch(ICommand command, CancellationToken cancellationToken = default);

    //todo: verify that this command can be executed using the ICommand syntax, by checking if the command is an MightRequireUserConfirmationCommand and if so, handle it accordingly. If the command is not a MightRequireUserConfirmationCommand, then it should be handled as a regular ICommand.
    //todo:
    //ValueTask<Result<CommandResponse>> Dispatch(MightRequireUserConfirmationCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle the given create command and return the result of the command execution.
    /// The result can be a success or a failure, and contains the key of the entity created.
    /// It may also contain additional information about the execution, such as an undo description or a toast message content.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the created entity. </typeparam>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command execution.</returns>
    Task<Result<CreateCommandResponse<TKey>>> Dispatch<TKey>(ICreateCommand<TKey> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handle the given query and return the result of the execution.
    /// The result can be a success or a failure, and contains the result of the query if it is a success.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the query. </typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query execution.</returns>
    Task<Result<TResult>> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
