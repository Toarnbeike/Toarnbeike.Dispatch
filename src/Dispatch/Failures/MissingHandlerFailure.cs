using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Failures;

public record MissingHandlerFailure<TRequest> : Failure
{
    public TRequest Request { get; }

    public MissingHandlerFailure(TRequest request)
    {
        Request = request;
        Message = $"No handler found for request of type {typeof(TRequest).FullName}";

    }
}