using System.Collections.Generic;
using System.Linq;

namespace Academy.Users.Application.Users.Commands.UpdateUser;

public class UpdateUserResult
{
    UpdateUserResult(bool isSuccess, UpdateUserResultType resultType, UpdateUserResponse? response, IReadOnlyCollection<string> errors, string message)
    {
        IsSuccess = isSuccess;
        ResultType = resultType;
        Response = response;
        Errors = errors;
        Message = message;
    }

    public bool IsSuccess { get; }
    public UpdateUserResultType ResultType { get; }
    public UpdateUserResponse? Response { get; }
    public IReadOnlyCollection<string> Errors { get; }
    public string Message { get; }

    public static UpdateUserResult Success(UpdateUserResponse response, string message)
    {
        return new UpdateUserResult(true, UpdateUserResultType.Success, response, Array.Empty<string>(), message);
    }

    public static UpdateUserResult ValidationFailure(IEnumerable<string> errors, string message)
    {
        return new UpdateUserResult(false, UpdateUserResultType.ValidationFailure, null, errors.ToArray(), message);
    }

    public static UpdateUserResult UserNotFound(string message)
    {
        return new UpdateUserResult(false, UpdateUserResultType.UserNotFound, null, Array.Empty<string>(), message);
    }

    public static UpdateUserResult PersistenceFailure(string message)
    {
        return new UpdateUserResult(false, UpdateUserResultType.PersistenceFailure, null, Array.Empty<string>(), message);
    }
}
