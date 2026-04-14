using System.ComponentModel;
using System.Windows.Input;

namespace Toarnbeike.Dispatch.Abstractions;

/// <summary>
/// Base interface for all requests.
/// Do not use externally, use <see cref="ICommand"/> or <see cref="IQuery{TResult}"/> instead.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IRequest<TResponse>;