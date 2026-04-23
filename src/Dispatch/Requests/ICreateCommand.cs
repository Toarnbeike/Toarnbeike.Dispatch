using Toarnbeike.Dispatch.Abstractions;
using Toarnbeike.Dispatch.Responses;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Request that commands the system to create a new entity.
/// Returns a <see cref="CreateCommandResponse{TKey}"/>, containing information regarding the created entity.
/// </summary>
/// <typeparam name="TKey">The key type of the entity created.</typeparam>
public interface ICreateCommand<TKey> : IRequest<CreateCommandResponse<TKey>>, ICommand
    where TKey : notnull;