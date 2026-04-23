using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Request that commands the system to create a new entity and returns the identifier of the created entity.
/// </summary>
/// <typeparam name="TCreateCommand">The type of the create command this handler processes.</typeparam>
/// <typeparam name="TKey">The key type of the entity created.</typeparam>
public interface ICreateCommandHandler<in TCreateCommand, TKey> : IRequestHandler<TCreateCommand, CreateCommandResponse<TKey>>
    where TCreateCommand : ICreateCommand<TKey>
    where TKey : notnull;