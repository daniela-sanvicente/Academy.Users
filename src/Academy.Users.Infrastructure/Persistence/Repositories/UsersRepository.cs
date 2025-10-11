using Academy.Users.Application.Users;
using Academy.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Academy.Users.Infrastructure.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    public UsersRepository(AcademyUsersDbContext context)
    {
        Context = context;
    }

    private AcademyUsersDbContext Context { get; }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await Context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            Context.Users.Update(user);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
