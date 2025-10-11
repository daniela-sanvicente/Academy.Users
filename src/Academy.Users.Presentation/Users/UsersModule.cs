using Academy.Users.Application.Users.Commands.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academy.Users.Presentation.Users;

public static class UsersModule
{
    public static IEndpointRouteBuilder MapUsersModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/users/{userId:int}", HandleUpdateUser)
            .WithSummary("Updates an existing user's personal data.")
            .WithDescription("Allows modifying first name, last name, phone number or address for the specified userId. At least one field must be provided in the request body.")
            .WithOpenApi(operation =>
            {
                operation.OperationId = "UpdateUserPersonalInformation";
                operation.Summary = "Updates the personal data of a user.";
                operation.Description = "Receives a JSON payload with firstName, lastName, phoneNumber and address, validates the format and persists the changes in the database.";
                return operation;
            });

        return endpoints;
    }

    internal static async Task<IResult> HandleUpdateUser(int userId, UpdateUserRequestDto request, ISender sender, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            var errorPayload = new ErrorResponseDto
            {
                Status = "InvalidData",
                Message = "The request body is required."
            };

            return Results.BadRequest(errorPayload);
        }

        var command = new UpdateUserCommand(userId, request.FirstName, request.LastName, request.PhoneNumber, request.Address);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess && result.Response is not null)
        {
            var successPayload = new UpdateUserSuccessResponseDto
            {
                UserId = result.Response.UserId,
                FirstName = result.Response.FirstName,
                LastName = result.Response.LastName,
                PhoneNumber = result.Response.PhoneNumber,
                Address = result.Response.Address,
                Status = result.Response.Status,
                Message = result.Response.Message
            };

            return Results.Ok(successPayload);
        }

        if (result.ResultType == UpdateUserResultType.ValidationFailure)
        {
            var validationPayload = new ValidationErrorResponseDto
            {
                Status = "InvalidData",
                Message = result.Message,
                Errors = result.Errors
            };

            return Results.BadRequest(validationPayload);
        }

        if (result.ResultType == UpdateUserResultType.UserNotFound)
        {
            var userNotFoundPayload = new ErrorResponseDto
            {
                Status = "UserNotFound",
                Message = result.Message
            };

            return Results.BadRequest(userNotFoundPayload);
        }

        var failurePayload = new ErrorResponseDto
        {
            Status = "ServerError",
            Message = result.Message
        };

        return Results.Json(failurePayload, statusCode: StatusCodes.Status500InternalServerError);
    }
}
