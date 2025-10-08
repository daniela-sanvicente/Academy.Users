using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Academy.Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<DependencyInjection>());
        return services;
    }
}
