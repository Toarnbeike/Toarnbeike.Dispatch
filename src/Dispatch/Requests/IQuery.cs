using Toarnbeike.Dispatch.Abstractions;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Request that queries the system for data and returns a result of type TResult.
/// </summary>
/// <typeparam name="TResult">The type of the returned result.</typeparam>
public interface IQuery<TResult> : IRequest<TResult>
    where TResult : notnull;