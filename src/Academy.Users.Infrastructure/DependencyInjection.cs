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
        var provider = configuration["DatabaseOptions:Provider"]?.Trim();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");
        }

        services.AddDbContext<AcademyUsersDbContext>(options => ConfigureProvider(options, provider, connectionString));
        services.AddScoped<IUsersRepository, UsersRepository>();
        return services;
    }

    private static void ConfigureProvider(DbContextOptionsBuilder options, string? provider, string connectionString)
    {
        switch (provider?.ToLowerInvariant())
        {
            case "sqlserver":
                options.UseSqlServer(connectionString);
                break;
            case "sqlserverexpress":
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
                break;
            case "sqlite":
            default:
                options.UseSqlite(connectionString);
                break;
        }
    }
}
