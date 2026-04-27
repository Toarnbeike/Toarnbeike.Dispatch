using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Request that commands the system to perform an action that modifies the state of the system.
/// Returns a <see cref="CommandResponse"/> containing information regarding the performed action, and an undo description that can be used to undo the action.
/// </summary>
public interface ICommand : IRequest<CommandResponse>;