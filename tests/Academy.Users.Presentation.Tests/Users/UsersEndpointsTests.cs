using Academy.Users.Application.Users.Commands.UpdateUser;
using Academy.Users.Presentation.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Academy.Users.Presentation.Tests.Users;

public class UsersModuleTests
{
    [Fact]
    public async Task HandleUpdateUser_WhenRequestIsNull_ReturnsBadRequest()
    {
        var sender = new FakeSender();

        var result = await UsersModule.HandleUpdateUser(1, null!, sender, CancellationToken.None);
        var httpResult = await ExecuteResultAsync(result);

        Assert.Equal(StatusCodes.Status400BadRequest, httpResult.StatusCode);
        Assert.Contains("\"status\":\"InvalidData\"", httpResult.Body);
        Assert.Null(sender.ReceivedCommand);
    }

    [Fact]
    public async Task HandleUpdateUser_WhenValidationFails_ReturnsBadRequestWithErrors()
    {
        var validationResult = UpdateUserResult.ValidationFailure(new[] { "error" }, "Invalid user data.");
        var sender = new FakeSender { ResultToReturn = validationResult };
        var request = new UpdateUserRequestDto { PhoneNumber = "123" };

        var result = await UsersModule.HandleUpdateUser(2, request, sender, CancellationToken.None);
        var httpResult = await ExecuteResultAsync(result);

        Assert.Equal(StatusCodes.Status400BadRequest, httpResult.StatusCode);
        Assert.Contains("\"status\":\"InvalidData\"", httpResult.Body);
        Assert.Equal(2, sender.ReceivedCommand?.UserId);
    }

    [Fact]
    public async Task HandleUpdateUser_WhenUserNotFound_ReturnsBadRequest()
    {
        var sender = new FakeSender { ResultToReturn = UpdateUserResult.UserNotFound("User not found.") };
        var request = new UpdateUserRequestDto { FirstName = "Ana" };

        var result = await UsersModule.HandleUpdateUser(9, request, sender, CancellationToken.None);
        var httpResult = await ExecuteResultAsync(result);

        Assert.Equal(StatusCodes.Status400BadRequest, httpResult.StatusCode);
        Assert.Contains("\"status\":\"UserNotFound\"", httpResult.Body);
        Assert.Equal(9, sender.ReceivedCommand?.UserId);
    }

    [Fact]
    public async Task HandleUpdateUser_WhenUpdateSucceeds_ReturnsOk()
    {
        var response = new UpdateUserResponse(1, "Ana", "Lopez", "5511122233", "Direccion", "ACTIVE", "ok");
        var sender = new FakeSender { ResultToReturn = UpdateUserResult.Success(response, "ok") };
        var request = new UpdateUserRequestDto { FirstName = "Ana" };

        var result = await UsersModule.HandleUpdateUser(1, request, sender, CancellationToken.None);
        var httpResult = await ExecuteResultAsync(result);

        Assert.Equal(StatusCodes.Status200OK, httpResult.StatusCode);
        Assert.Contains("\"userId\":1", httpResult.Body);
        Assert.Equal(1, sender.ReceivedCommand?.UserId);
    }

    [Fact]
    public async Task HandleUpdateUser_WhenPersistenceFails_ReturnsServerError()
    {
        var sender = new FakeSender { ResultToReturn = UpdateUserResult.PersistenceFailure("fail") };
        var request = new UpdateUserRequestDto { FirstName = "Ana" };

        var result = await UsersModule.HandleUpdateUser(3, request, sender, CancellationToken.None);
        var httpResult = await ExecuteResultAsync(result);

        Assert.Equal(StatusCodes.Status500InternalServerError, httpResult.StatusCode);
        Assert.Contains("\"status\":\"ServerError\"", httpResult.Body);
        Assert.Equal(3, sender.ReceivedCommand?.UserId);
    }

    private static async Task<(int StatusCode, string Body)> ExecuteResultAsync(IResult result)
    {
        var context = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.Configure<JsonOptions>(_ => { });
        context.RequestServices = services.BuildServiceProvider();
        context.Response.Body = new MemoryStream();
        await result.ExecuteAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var payload = await reader.ReadToEndAsync();
        return (context.Response.StatusCode, payload);
    }

    private sealed class FakeSender : ISender
    {
        public UpdateUserResult ResultToReturn { get; set; } = UpdateUserResult.Success(new UpdateUserResponse(0, "", "", "", "", "", ""), "");
        public UpdateUserCommand? ReceivedCommand { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            if (request is UpdateUserCommand command)
            {
                ReceivedCommand = command;
                return Task.FromResult((TResponse)(object)ResultToReturn);
            }

            return Task.FromResult(default(TResponse)!);
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken)
        {
            if (request is UpdateUserCommand command)
            {
                ReceivedCommand = command;
                return Task.FromResult<object?>(ResultToReturn);
            }

            return Task.FromResult<object?>(null);
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return Empty<TResponse>();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken)
        {
            return Empty<object?>();
        }

        private static async IAsyncEnumerable<T> Empty<T>()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
