using Toarnbeike.Dispatch.Failures;
using Toarnbeike.Dispatch.Responses;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Handler for processing commands that might require user confirmation before being executed.
/// </summary>
/// <typeparam name="TCommand">The type of the command this handler processes.</typeparam>
public abstract class MightRequireUserConfirmationCommandHandler<TCommand> : IRequestHandler<TCommand, CommandResponse>
    where TCommand : MightRequireUserConfirmationCommand, IRequest<CommandResponse>
{
    /// <summary>
    /// Determine if the command requires user confirmation before being executed.
    /// This allows for commands to be executed immediately if they are already confirmed, or if they do not require confirmation, while still supporting a confirmation flow for commands that do require it.
    /// </summary>
    /// <param name="command">The command to check for confirmation requirement.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether confirmation is required.</returns>
    protected abstract Task<bool> IsConfirmationRequired(TCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Build the message that is shown to the user when asking for confirmation. This allows for dynamic confirmation messages that can include details from the command, providing a better user experience and reducing the risk of users confirming actions they did not intend to.
    /// For example, a command to delete an item could include the name of the item in the confirmation message, making it clear what action the user is confirming.
    /// </summary>
    /// <param name="command">The command for which to create the confirmation message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the confirmation message.</returns>
    protected abstract Task<string> CreateConfirmationMessage(TCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handler implementation for the command, after it has been confirmed or determined confirmation is not required.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the request handling.</returns>
    protected abstract ValueTask<Result<CommandResponse>> HandleConfirmed(TCommand command, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public async Task<Result<CommandResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken) =>
        command.IsConfirmed || !await IsConfirmationRequired(command, cancellationToken)
            ? await HandleConfirmed(command, cancellationToken)
            : new ConfirmationRequiredFailure(
                command with {IsConfirmed = true},
                await CreateConfirmationMessage(command, cancellationToken));
}