namespace Toarnbeike.Dispatch.Responses;

/// <summary>
/// Response for a create command request, containing information about the created key, the performed action, and an undo description that can be used to undo the action.
/// Create commands always contain information for a toast notification, such as the message and title of the toast.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <param name="CreatedEntityId">Id of the entity created.</param>
/// <param name="Description">Short message for the Undo button tooltip.</param>
/// <param name="ToastMessage">The message to be displayed in the toast notification.</param>
/// <param name="ToastTitle">The title of the toast notification.</param>
public record CreateCommandResponse<TKey>(
    TKey CreatedEntityId,
    string Description,
    string ToastMessage,
    string ToastTitle) : ToasterCommandResponse(Description, ToastMessage, ToastTitle);