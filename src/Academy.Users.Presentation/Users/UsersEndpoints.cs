using System.Collections.Generic;
using Academy.Users.Application.Users.Commands.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Academy.Users.Presentation.Users;

    public static class UsersEndpoints
    {
        public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/users/{userId:int}", HandleUpdateUser)
                .WithName("UpdateUserPersonalInformation")
                .WithTags("Usuarios")
                .WithSummary("Actualiza los datos personales de un usuario existente.")
                .WithDescription("Permite modificar nombre, apellido, número telefónico y dirección de un usuario identificado por el parámetro userId. Al menos uno de los campos debe ser proporcionado en el cuerpo.")
                .Accepts<UpdateUserRequestDto>("application/json")
                .Produces<UpdateUserSuccessResponseDto>(StatusCodes.Status200OK)
                .Produces<ValidationErrorResponseDto>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponseDto>(StatusCodes.Status500InternalServerError)
                .WithOpenApi(operation =>
                {
                    operation.OperationId = "UpdateUserPersonalInformation";
                    operation.Summary = "Actualiza los datos personales de un usuario.";
                    operation.Description = "Recibe un cuerpo JSON con los campos firstName, lastName, phoneNumber y address, valida el formato y guarda los cambios en la base de datos. " +
                                             "Reglas de validación: (1) los campos enviados no pueden ser cadenas vacías, (2) el número telefónico debe ser mexicano (10 dígitos o prefijo +52), " +
                                             "(3) al menos un campo debe suministrarse, (4) el usuario debe existir.";
                    operation.Responses[StatusCodes.Status200OK.ToString()].Description = "Actualización exitosa.";
                    operation.Responses[StatusCodes.Status400BadRequest.ToString()].Description = "Solicitud inválida o usuario inexistente.";
                    operation.Responses[StatusCodes.Status500InternalServerError.ToString()].Description = "Error interno al persistir los cambios.";

                    if (operation.RequestBody is not null)
                    {
                        operation.RequestBody.Description = "Campos opcionales que permiten actualizar uno o varios datos personales del usuario.";
                    }

                    if (operation.RequestBody?.Content.TryGetValue("application/json", out var requestContent) == true)
                    {
                        requestContent.Example = new OpenApiObject
                        {
                            ["firstName"] = new OpenApiString("Ana María"),
                            ["lastName"] = new OpenApiString("López Hernández"),
                            ["phoneNumber"] = new OpenApiString("+525511122233"),
                            ["address"] = new OpenApiString("Av. Reforma 500, Piso 12, Ciudad de México")
                        };
                    }

                    if (operation.Responses.TryGetValue(StatusCodes.Status200OK.ToString(), out var successResponse) &&
                        successResponse.Content.TryGetValue("application/json", out var successMediaType))
                    {
                        successMediaType.Example = new OpenApiObject
                        {
                            ["userId"] = new OpenApiInteger(1),
                            ["firstName"] = new OpenApiString("Ana María"),
                            ["lastName"] = new OpenApiString("López Hernández"),
                            ["phoneNumber"] = new OpenApiString("+525511122233"),
                            ["address"] = new OpenApiString("Av. Reforma 500, Piso 12, Ciudad de México"),
                            ["status"] = new OpenApiString("ACTIVE"),
                            ["message"] = new OpenApiString("User information updated successfully.")
                        };
                    }

                    if (operation.Responses.TryGetValue(StatusCodes.Status400BadRequest.ToString(), out var badRequestResponse) &&
                        badRequestResponse.Content.TryGetValue("application/json", out var badRequestMediaType))
                    {
                        badRequestMediaType.Examples = new Dictionary<string, OpenApiExample>
                        {
                            ["ValidationError"] = new OpenApiExample
                            {
                                Summary = "Errores de validación en los datos enviados",
                                Value = new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("InvalidData"),
                                    ["message"] = new OpenApiString("Invalid user data."),
                                    ["errors"] = new OpenApiArray
                                    {
                                        new OpenApiString("Phone number is not valid for Mexico.")
                                    }
                                }
                            },
                            ["EmptyFields"] = new OpenApiExample
                            {
                                Summary = "Campos enviados pero vacíos después del trim",
                                Value = new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("InvalidData"),
                                    ["message"] = new OpenApiString("Invalid user data."),
                                    ["errors"] = new OpenApiArray
                                    {
                                        new OpenApiString("First name cannot be empty."),
                                        new OpenApiString("Last name cannot be empty."),
                                        new OpenApiString("Address cannot be empty.")
                                    }
                                }
                            },
                            ["NoFieldsProvided"] = new OpenApiExample
                            {
                                Summary = "No se proporcionó ningún campo para actualizar",
                                Value = new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("InvalidData"),
                                    ["message"] = new OpenApiString("Invalid user data."),
                                    ["errors"] = new OpenApiArray
                                    {
                                        new OpenApiString("No fields were provided for update.")
                                    }
                                }
                            },
                            ["UserNotFound"] = new OpenApiExample
                            {
                                Summary = "Usuario inexistente para el userId solicitado",
                                Value = new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("UserNotFound"),
                                    ["message"] = new OpenApiString("User not found.")
                                }
                            }
                        };
                    }

                    if (operation.Responses.TryGetValue(StatusCodes.Status500InternalServerError.ToString(), out var serverErrorResponse) &&
                        serverErrorResponse.Content.TryGetValue("application/json", out var serverErrorMediaType))
                    {
                        serverErrorMediaType.Example = new OpenApiObject
                        {
                            ["status"] = new OpenApiString("ServerError"),
                            ["message"] = new OpenApiString("Could not update user information.")
                        };
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
