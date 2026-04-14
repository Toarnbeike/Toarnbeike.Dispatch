namespace Toarnbeike.Dispatch.Responses;

/// <summary>
/// Response for a command request, containing information regarding the performed action, and an undo description that can be used to undo the action.
/// </summary>
/// <param name="Description">Short message for the Undo button tooltip.</param>
public record CommandResponse(
    string Description)
{
    public override string ToString() => Description;
}