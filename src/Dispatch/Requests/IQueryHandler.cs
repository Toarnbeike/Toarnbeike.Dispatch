using Toarnbeike.Dispatch.Abstractions;

namespace Toarnbeike.Dispatch.Requests;

/// <summary>
/// Handler for processing queries.
/// </summary>
/// <typeparam name="TQuery">The type of the query this handler processes</typeparam>
/// <typeparam name="TResult">The result (payload) of the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
    where TResult : notnull;