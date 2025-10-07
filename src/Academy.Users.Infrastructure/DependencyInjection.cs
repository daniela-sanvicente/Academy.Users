using Academy.Users.Application.Users;
using Academy.Users.Infrastructure.Persistence;
using Academy.Users.Infrastructure.Persistence.Repositories;
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
        var resolvedConnectionString = hasConnectionString ? connectionString! : "Data Source=academy_users_local.db";
        services.AddDbContext<AcademyUsersDbContext>(options => options.UseSqlite(resolvedConnectionString));
        services.AddScoped<IUsersRepository, UsersRepository>();
        return services;
    }
}
