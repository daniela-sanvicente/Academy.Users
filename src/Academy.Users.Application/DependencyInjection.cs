using Academy.Users.Application.Users.Commands.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Academy.Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUpdateUserPersonalInformationService, UpdateUserPersonalInformationService>();
        return services;
    }
}
