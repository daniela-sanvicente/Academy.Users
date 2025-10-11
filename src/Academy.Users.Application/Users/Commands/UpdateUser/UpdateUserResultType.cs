namespace Academy.Users.Application.Users.Commands.UpdateUser;

public enum UpdateUserResultType
{
    Success,
    ValidationFailure,
    UserNotFound,
    PersistenceFailure
}
