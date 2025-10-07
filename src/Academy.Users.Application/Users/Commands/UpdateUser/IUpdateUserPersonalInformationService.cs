namespace Academy.Users.Application.Users.Commands.UpdateUser;

public interface IUpdateUserPersonalInformationService
{
    Task<UpdateUserResult> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}
