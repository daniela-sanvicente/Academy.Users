namespace Academy.Users.Application.Users.Commands.UpdateUser;

public record UpdateUserResponse(int UserId, string FirstName, string LastName, string PhoneNumber, string Address, string Status, string Message);
