using Academy.Users.Domain.Users;

namespace Academy.Users.Application.Users;

public interface IUsersRepository
{
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
}
