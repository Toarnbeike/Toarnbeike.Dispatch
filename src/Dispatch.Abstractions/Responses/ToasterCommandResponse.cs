namespace Toarnbeike.Dispatch.Responses;

/// <summary>
/// Response for a command request, containing information regarding the performed action, and an undo description that can be used to undo the action.
/// Also contains information for a toast notification, such as the message and title of the toast.
/// </summary>
/// <param name="Description">Short message for the Undo button tooltip.</param>
/// <param name="ToastMessage">The message to be displayed in the toast notification.</param>
/// <param name="ToastTitle">The title of the toast notification.</param>
public record ToasterCommandResponse(
    string Description,
    string ToastMessage,
    string ToastTitle)
    : CommandResponse(Description);