using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Failures;

public record RedoUnavailableFailure : Failure
{
    public RedoUnavailableFailure()
    {
        Message = "Redo is unavailable for this mutation.";
    }
}