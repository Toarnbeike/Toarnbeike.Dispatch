using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Handler for processing commands.
/// </summary>
/// <typeparam name="TCommand">The type of the command this handler processes</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, CommandResponse>
    where TCommand : ICommand;