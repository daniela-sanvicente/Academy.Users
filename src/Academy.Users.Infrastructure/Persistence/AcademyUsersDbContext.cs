using Academy.Users.Domain.Users;
using Academy.Users.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Academy.Users.Infrastructure.Persistence;

public class AcademyUsersDbContext : DbContext
{
    public AcademyUsersDbContext(DbContextOptions<AcademyUsersDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
