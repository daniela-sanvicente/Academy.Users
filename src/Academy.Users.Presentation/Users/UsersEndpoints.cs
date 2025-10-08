using Academy.Users.Application.Users.Commands.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace Academy.Users.Presentation.Users;

/// <summary>
/// Conjunto de endpoints relacionados con la administración de usuarios.
/// </summary>
public static class UsersEndpoints
{
    /// <summary>
    /// Permite al cliente actualizar parcial o totalmente sus datos personales mediante el recurso PUT /users/{userId}.
    /// </summary>
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/users/{userId:int}", HandleUpdateUser)
        .WithTags("Usuarios")
        .Accepts<UpdateUserRequestDto>("application/json")
        .Produces<UpdateUserSuccessResponseDto>(StatusCodes.Status200OK)
        .Produces<ValidationErrorResponseDto>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponseDto>(StatusCodes.Status500InternalServerError)
        .WithSummary("Actualiza los datos personales de un usuario existente.")
        .WithDescription("Permite modificar nombre, apellido, número telefónico y dirección de un usuario identificado por el parámetro userId.")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Actualiza los datos personales de un usuario.";
            operation.Description = "Recibe un cuerpo JSON con los campos firstName, lastName, phoneNumber y address, valida el formato y guarda los cambios en la base de datos. Responde con el estado actualizado o con mensajes de error en caso de datos inválidos o fallos internos.";
            operation.Responses[StatusCodes.Status200OK.ToString()].Description = "Actualización exitosa.";
            operation.Responses[StatusCodes.Status400BadRequest.ToString()].Description = "Solicitud inválida o usuario inexistente.";
            operation.Responses[StatusCodes.Status500InternalServerError.ToString()].Description = "Error interno al persistir los cambios.";

            if (operation.RequestBody?.Content.TryGetValue("application/json", out var requestContent) == true)
            {
                requestContent.Example = new OpenApiString("{\n  \"firstName\": \"Ana María\",\n  \"lastName\": \"López Hernández\",\n  \"phoneNumber\": \"+525511122233\",\n  \"address\": \"Av. Reforma 500, Piso 12, Ciudad de México\"\n}");
            }

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
