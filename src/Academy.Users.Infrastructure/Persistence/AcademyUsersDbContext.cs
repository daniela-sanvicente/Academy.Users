using Microsoft.EntityFrameworkCore;

namespace Academy.Users.Infrastructure.Persistence;

public class AcademyUsersDbContext : DbContext
{
    public AcademyUsersDbContext(DbContextOptions<AcademyUsersDbContext> options) : base(options)
    {
    }
}
