using Academy.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academy.Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var hasConnectionString = string.IsNullOrWhiteSpace(connectionString) == false;
        if (hasConnectionString)
        {
            services.AddDbContext<AcademyUsersDbContext>(options => options.UseSqlite(connectionString));
        }

        return services;
    }
}
