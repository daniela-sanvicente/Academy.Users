using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Academy.Users.Application.Users.Commands.UpdateUser;
using Academy.Users.Presentation.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Academy.Users.Presentation.Tests.Users;

public class UsersEndpointsTests
{
    [Fact]
    public async Task MapUsersEndpoints_WhenUpdateSucceeds_ReturnsOk()
    {
        var service = new FakeUpdateUserPersonalInformationService
        {
            ResultToReturn = UpdateUserResult.Success(new UpdateUserResponse(1, "Ana", "Lopez", "5511122233", "Direccion", "ACTIVE", "ok"), "ok")
        };
        var (endpoint, app) = CreateEndpoint(service);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/1", """{"firstName":"Ana"}""");

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        var body = await ReadBodyAsync(context);
        Assert.Contains("\"userId\":1", body);
        Assert.Equal(1, service.ReceivedCommand?.UserId);
    }

    [Fact]
    public async Task MapUsersEndpoints_WhenValidationFails_ReturnsBadRequest()
    {
        var result = UpdateUserResult.ValidationFailure(new[] { "error" }, "Invalid");
        var service = new FakeUpdateUserPersonalInformationService { ResultToReturn = result };
        var (endpoint, app) = CreateEndpoint(service);
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
        var service = new FakeUpdateUserPersonalInformationService
        {
            ResultToReturn = UpdateUserResult.UserNotFound("missing")
        };
        var (endpoint, app) = CreateEndpoint(service);
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
        var service = new FakeUpdateUserPersonalInformationService
        {
            ResultToReturn = UpdateUserResult.PersistenceFailure("fail")
        };
        var (endpoint, app) = CreateEndpoint(service);
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
        var service = new FakeUpdateUserPersonalInformationService
        {
            ResultToReturn = UpdateUserResult.Success(new UpdateUserResponse(1, "", "", "", "", "", ""), "")
        };
        var (endpoint, app) = CreateEndpoint(service);
        await using var _ = app;
        using var scope = app.Services.CreateScope();
        var context = CreateHttpContext(scope.ServiceProvider, endpoint, "/api/v1/users/4", null);

        await endpoint.RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Null(service.ReceivedCommand);
    }

    private static (RouteEndpoint Endpoint, WebApplication App) CreateEndpoint(IUpdateUserPersonalInformationService service)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(service);
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

    private sealed class FakeUpdateUserPersonalInformationService : IUpdateUserPersonalInformationService
    {
        public UpdateUserResult ResultToReturn { get; set; } = UpdateUserResult.Success(new UpdateUserResponse(0, "", "", "", "", "", ""), "");
        public UpdateUserCommand? ReceivedCommand { get; private set; }

        public Task<UpdateUserResult> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            ReceivedCommand = command;
            return Task.FromResult(ResultToReturn);
        }
    }
}
