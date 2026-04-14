using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Request that commands the system to perform an action that modifies the state of the system.
/// Handling such a command may require user confirmation, which is determined by the handler of the command.
/// If user confirmation is required, the handler will return a failure containing a confirmation message, and the command with the IsConfirmed property set to true.
/// The caller can then display the confirmation message to the user, and if the user confirms, resend the command with the IsConfirmed property set to true.
/// Returns a <see cref="CommandResponse"/> containing information regarding the performed action, and an undo description that can be used to undo the action.
/// </summary>
public abstract record MightRequireUserConfirmationCommand : ICommand
{
    internal bool IsConfirmed { get; init; } = false;
}