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
                .WithSummary("Actualiza los datos personales de un usuario existente.")
                .WithDescription("Permite modificar nombre, apellido, número telefónico y dirección de un usuario identificado por el parámetro userId. Al menos uno de los campos debe ser proporcionado en el cuerpo.")
                .WithOpenApi(operation =>
                {
                    operation.OperationId = "UpdateUserPersonalInformation";
                    operation.Summary = "Actualiza los datos personales de un usuario.";
                    operation.Description = "Recibe un cuerpo JSON con los campos firstName, lastName, phoneNumber y address, valida el formato y guarda los cambios en la base de datos.";
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
                Message = "El cuerpo de la solicitud es obligatorio."
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
