using MediatR;

namespace Academy.Users.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(int UserId, string? FirstName, string? LastName, string? PhoneNumber, string? Address) : IRequest<UpdateUserResult>;
