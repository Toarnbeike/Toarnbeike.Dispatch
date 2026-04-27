using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Pipelines;

public delegate Task<Result<TResult>> RequestHandlerDelegate<TResult>();
