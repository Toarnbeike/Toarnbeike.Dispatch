using Toarnbeike.Dispatch.Requests;
using Toarnbeike.Results;

namespace Toarnbeike.Dispatch.Failures;

public record ConfirmationRequiredFailure : Failure
{
    public ICommand ConfirmedCommand { get; }

    public ConfirmationRequiredFailure(ICommand confirmedCommand, string message)
    {
        ConfirmedCommand = confirmedCommand;
        Message = message;
    }
}