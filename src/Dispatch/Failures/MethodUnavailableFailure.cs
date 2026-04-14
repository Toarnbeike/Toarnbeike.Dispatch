using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Failures;

public record MethodUnavailableFailure : Failure
{
    public MethodUnavailableFailure(string methodName)
    {
        Message =
            $"{methodName} is unavailable at this moment. Check whether the method is available using the 'Can{methodName}' property.";
    }
}