using System.Collections.Generic;
using System.Linq;
using Academy.Users.Application.Users;
using Academy.Users.Domain.Users;

namespace Academy.Users.Application.Users.Commands.UpdateUser;

public class UpdateUserPersonalInformationService : IUpdateUserPersonalInformationService
{
    public UpdateUserPersonalInformationService(IUsersRepository usersRepository)
    {
        UsersRepository = usersRepository;
    }

    private IUsersRepository UsersRepository { get; }

    public async Task<UpdateUserResult> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var validationErrors = new List<string>();
        var sanitizedFirstName = command.FirstName?.Trim();
        if (sanitizedFirstName is not null && sanitizedFirstName.Length == 0)
        {
            validationErrors.Add("First name cannot be empty.");
            sanitizedFirstName = null;
        }

        var sanitizedLastName = command.LastName?.Trim();
        if (sanitizedLastName is not null && sanitizedLastName.Length == 0)
        {
            validationErrors.Add("Last name cannot be empty.");
            sanitizedLastName = null;
        }

        var sanitizedAddress = command.Address?.Trim();
        if (sanitizedAddress is not null && sanitizedAddress.Length == 0)
        {
            validationErrors.Add("Address cannot be empty.");
            sanitizedAddress = null;
        }

        string? normalizedPhoneNumber = null;
        if (command.PhoneNumber is not null)
        {
            var sanitizedPhoneNumber = command.PhoneNumber.Trim();
            if (sanitizedPhoneNumber.Length == 0)
            {
                validationErrors.Add("Phone number cannot be empty.");
            }
            else
            {
                var isValidPhoneNumber = IsValidMexicanPhoneNumber(sanitizedPhoneNumber);
                if (isValidPhoneNumber == false)
                {
                    validationErrors.Add("Phone number is not valid for Mexico.");
                }
                else
                {
                    normalizedPhoneNumber = NormalizePhoneNumber(sanitizedPhoneNumber);
                }
            }
        }

        var hasAnyFieldProvided = command.FirstName is not null || command.LastName is not null || command.PhoneNumber is not null || command.Address is not null;
        if (hasAnyFieldProvided == false)
        {
            validationErrors.Add("No fields were provided for update.");
        }

        if (validationErrors.Any())
        {
            return UpdateUserResult.ValidationFailure(validationErrors, "Invalid user data.");
        }

        var user = await UsersRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return UpdateUserResult.UserNotFound("User not found.");
        }

        var hasChanges = false;
        if (sanitizedFirstName is not null && sanitizedFirstName != user.FirstName)
        {
            user.FirstName = sanitizedFirstName;
            hasChanges = true;
        }

        if (sanitizedLastName is not null && sanitizedLastName != user.LastName)
        {
            user.LastName = sanitizedLastName;
            hasChanges = true;
        }

        if (sanitizedAddress is not null && sanitizedAddress != user.Address)
        {
            user.Address = sanitizedAddress;
            hasChanges = true;
        }

        if (normalizedPhoneNumber is not null && normalizedPhoneNumber != user.PhoneNumber)
        {
            user.PhoneNumber = normalizedPhoneNumber;
            hasChanges = true;
        }

        if (hasChanges == false)
        {
            var noChangeResponse = new UpdateUserResponse(user.Id, user.FirstName, user.LastName, user.PhoneNumber, user.Address, user.Status, "No changes were applied.");
            return UpdateUserResult.Success(noChangeResponse, "No changes were applied.");
        }

        user.UpdatedAt = DateTime.UtcNow;
        var updateSucceeded = await UsersRepository.UpdateAsync(user, cancellationToken);
        if (updateSucceeded == false)
        {
            return UpdateUserResult.PersistenceFailure("Could not update user information.");
        }

        var response = new UpdateUserResponse(user.Id, user.FirstName, user.LastName, user.PhoneNumber, user.Address, user.Status, "User information updated successfully.");
        return UpdateUserResult.Success(response, "User information updated successfully.");
    }

    private static bool IsValidMexicanPhoneNumber(string phoneNumber)
    {
        var normalizedDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (normalizedDigits.Length == 10)
        {
            return true;
        }

        if (normalizedDigits.Length == 12 && normalizedDigits.StartsWith("52"))
        {
            return true;
        }

        return false;
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        var normalizedDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (normalizedDigits.Length == 10)
        {
            return normalizedDigits;
        }

        if (normalizedDigits.Length == 12 && normalizedDigits.StartsWith("52"))
        {
            return $"+{normalizedDigits}";
        }

        return phoneNumber.Trim();
    }
}
