using Academy.Users.Application.Users.Commands.UpdateUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academy.Users.Presentation.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/users/{userId:int}", async (int userId, UpdateUserRequestDto request, IUpdateUserPersonalInformationService service, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                var errorPayload = new { status = "InvalidData", message = "Request body is required." };
                return Results.BadRequest(errorPayload);
            }

            var command = new UpdateUserCommand(userId, request.FirstName, request.LastName, request.PhoneNumber, request.Address);
            var result = await service.UpdateAsync(command, cancellationToken);

            if (result.IsSuccess && result.Response is not null)
            {
                var successPayload = new
                {
                    userId = result.Response.UserId,
                    firstName = result.Response.FirstName,
                    lastName = result.Response.LastName,
                    phoneNumber = result.Response.PhoneNumber,
                    address = result.Response.Address,
                    status = result.Response.Status,
                    message = result.Response.Message
                };

                return Results.Ok(successPayload);
            }

            if (result.ResultType == UpdateUserResultType.ValidationFailure)
            {
                var validationPayload = new
                {
                    status = "InvalidData",
                    message = result.Message,
                    errors = result.Errors
                };
                return Results.BadRequest(validationPayload);
            }

            if (result.ResultType == UpdateUserResultType.UserNotFound)
            {
                var userNotFoundPayload = new { status = "UserNotFound", message = result.Message };
                return Results.BadRequest(userNotFoundPayload);
            }

            var failurePayload = new { status = "ServerError", message = result.Message };
            return Results.Json(failurePayload, statusCode: StatusCodes.Status500InternalServerError);
        });

        return endpoints;
    }
}
