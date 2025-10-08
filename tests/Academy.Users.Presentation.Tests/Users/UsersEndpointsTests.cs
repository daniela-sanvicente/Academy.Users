using Academy.Users.Application.Users.Commands.UpdateUser;
using Academy.Users.Presentation.Users;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Text;

namespace Academy.Users.Presentation.Tests.Users;

public class UsersEndpointsTests
{
    [Fact]
    public async Task MapUsersEndpoints_WhenUpdateSucceeds_ReturnsOk()
    {
        var sender = new FakeSender
        {
            ResultToReturn = UpdateUserResult.Success(new UpdateUserResponse(1, "Ana", "Lopez", "5511122233", "Direccion", "ACTIVE", "ok"), "ok")
        };
        var (endpoint, app) = CreateEndpoint(sender);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/1", """{"firstName":"Ana"}""");

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        var body = await ReadBodyAsync(context);
        Assert.Contains("\"userId\":1", body);
        Assert.Equal(1, sender.ReceivedCommand?.UserId);
    }

    [Fact]
    public async Task MapUsersEndpoints_WhenValidationFails_ReturnsBadRequest()
    {
        var result = UpdateUserResult.ValidationFailure(new[] { "error" }, "Invalid");
        var sender = new FakeSender { ResultToReturn = result };
        var (endpoint, app) = CreateEndpoint(sender);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/2", """{"phoneNumber":"123"}""");

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        var body = await ReadBodyAsync(context);
        Assert.Contains("\"status\":\"InvalidData\"", body);
    }

    [Fact]
    public async Task MapUsersEndpoints_WhenUserNotFound_ReturnsBadRequest()
    {
        var sender = new FakeSender
        {
            ResultToReturn = UpdateUserResult.UserNotFound("missing")
        };
        var (endpoint, app) = CreateEndpoint(sender);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/9", """{"firstName":"Ana"}""");

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        var body = await ReadBodyAsync(context);
        Assert.Contains("\"status\":\"UserNotFound\"", body);
    }

    [Fact]
    public async Task MapUsersEndpoints_WhenPersistenceFails_ReturnsServerError()
    {
        var sender = new FakeSender
        {
            ResultToReturn = UpdateUserResult.PersistenceFailure("fail")
        };
        var (endpoint, app) = CreateEndpoint(sender);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/3", """{"firstName":"Ana"}""");

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        var body = await ReadBodyAsync(context);
        Assert.Contains("\"status\":\"ServerError\"", body);
    }

    [Fact]
    public async Task MapUsersEndpoints_WhenBodyMissing_ReturnsBadRequestWithoutInvokingService()
    {
        var sender = new FakeSender
        {
            ResultToReturn = UpdateUserResult.Success(new UpdateUserResponse(1, "", "", "", "", "", ""), "")
        };
        var (endpoint, app) = CreateEndpoint(sender);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/4", null);

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Null(sender.ReceivedCommand);
    }

    private static (RouteEndpoint Endpoint, WebApplication App) CreateEndpoint(ISender sender)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(sender);
        var app = builder.Build();
        var group = app.MapGroup("/api/v1");
        group.MapUsersEndpoints();
        var endpointBuilder = (IEndpointRouteBuilder)app;
        var endpoint = endpointBuilder.DataSources
            .SelectMany(x => x.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(x => x.RoutePattern.RawText == "/api/v1/users/{userId:int}" && x.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Contains("PUT") == true);
        return (endpoint, app);
    }

    private static DefaultHttpContext CreateHttpContext(IServiceProvider serviceProvider, RouteEndpoint endpoint, string path, string? body)
    {
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        context.SetEndpoint(endpoint);
        context.Request.Method = "PUT";
        context.Request.Path = path;
        context.Request.RouteValues["userId"] = path.Split('/').Last();
        context.Response.Body = new MemoryStream();
        if (body is not null)
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            context.Request.Body = new MemoryStream(bytes);
            context.Request.ContentType = "application/json";
            context.Request.ContentLength = bytes.Length;
        }
        else
        {
            context.Request.Body = new MemoryStream();
            context.Request.ContentType = "application/json";
            context.Request.ContentLength = 0;
        }

        return context;
    }

    private static async Task<string> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
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
    }
}
