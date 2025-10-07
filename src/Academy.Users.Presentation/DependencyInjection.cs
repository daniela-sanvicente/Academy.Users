using Academy.Users.Presentation.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Academy.Users.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }

    public static WebApplication MapPresentationEndpoints(this WebApplication app)
    {
        app.MapUsersEndpoints();
        return app;
    }
}
