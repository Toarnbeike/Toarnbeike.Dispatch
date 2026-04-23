using System.ComponentModel;

namespace Toarnbeike.Dispatch.Abstractions;

/// <summary>
/// Base interface for all requests.
/// Do not use externally, use <c>ICommand</c> or <c>IQuery{TResult}</c> instead.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IRequest<TResponse> where TResponse : notnull;