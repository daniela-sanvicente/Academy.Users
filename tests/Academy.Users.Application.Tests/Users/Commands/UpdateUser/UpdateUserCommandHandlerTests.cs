using Academy.Users.Application.Users;
using Academy.Users.Application.Users.Commands.UpdateUser;
using Academy.Users.Domain.Users;

namespace Academy.Users.Application.Tests.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task UpdateAsync_WithNoFieldsProvided_ReturnsValidationFailure()
    {
        var repository = new FakeUsersRepository();
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(1, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.ValidationFailure, result.ResultType);
        Assert.Contains("No fields were provided for update.", result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyFields_ReturnsValidationFailure()
    {
        var repository = new FakeUsersRepository();
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(1, " ", " ", null, " ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.ValidationFailure, result.ResultType);
        Assert.Contains("First name cannot be empty.", result.Errors);
        Assert.Contains("Last name cannot be empty.", result.Errors);
        Assert.Contains("Address cannot be empty.", result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidPhoneNumber_ReturnsValidationFailure()
    {
        var repository = new FakeUsersRepository();
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(1, null, null, "12345", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.ValidationFailure, result.ResultType);
        Assert.Contains("Phone number is not valid for Mexico.", result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsUserNotFound()
    {
        var repository = new FakeUsersRepository();
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(42, "Ana", "Lopez", null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.UserNotFound, result.ResultType);
    }

    [Fact]
    public async Task UpdateAsync_WhenNoChanges_ReturnsSuccessWithoutUpdatingRepository()
    {
        var existingUser = new User
        {
            Id = 5,
            FirstName = "Ana",
            LastName = "Lopez",
            Email = "ana@example.com",
            PhoneNumber = "5511122233",
            Address = "Direccion",
            PasswordHash = "hash",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var repository = new FakeUsersRepository { UserToReturn = existingUser };
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(existingUser.Id, "Ana", "Lopez", "5511122233", "Direccion");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.Success, result.ResultType);
        Assert.NotNull(result.Response);
        Assert.Equal("No changes were applied.", result.Response!.Message);
        Assert.Null(repository.UpdatedUser);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateFails_ReturnsPersistenceFailure()
    {
        var existingUser = new User
        {
            Id = 7,
            FirstName = "Ana",
            LastName = "Lopez",
            Email = "ana@example.com",
            PhoneNumber = "5511122233",
            Address = "Direccion",
            PasswordHash = "hash",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var repository = new FakeUsersRepository { UserToReturn = existingUser, UpdateResult = false };
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(existingUser.Id, "Ana Maria", "Lopez", "5511122233", "Direccion");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.PersistenceFailure, result.ResultType);
    }

    [Fact]
    public async Task UpdateAsync_WithValidChanges_ReturnsSuccessAndUpdatesUser()
    {
        var existingUser = new User
        {
            Id = 9,
            FirstName = "Ana",
            LastName = "Lopez",
            Email = "ana@example.com",
            PhoneNumber = "5511122233",
            Address = "Direccion",
            PasswordHash = "hash",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var repository = new FakeUsersRepository { UserToReturn = existingUser };
        var handler = new UpdateUserCommandHandler(repository);
        var command = new UpdateUserCommand(existingUser.Id, " Ana Maria ", null, "+52 55 1112 2233", " Nueva direccion ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(UpdateUserResultType.Success, result.ResultType);
        Assert.NotNull(result.Response);
        Assert.Equal("Ana Maria", result.Response!.FirstName);
        Assert.Equal("+525511122233", result.Response.PhoneNumber);
        Assert.Equal("Nueva direccion", result.Response.Address);
        Assert.NotNull(repository.UpdatedUser);
        Assert.Equal("Ana Maria", repository.UpdatedUser!.FirstName);
        Assert.Equal("+525511122233", repository.UpdatedUser.PhoneNumber);
        Assert.Equal("Nueva direccion", repository.UpdatedUser.Address);
        Assert.True(repository.UpdatedUser.UpdatedAt > existingUser.UpdatedAt);
    }

    private sealed class FakeUsersRepository : IUsersRepository
    {
        public User? UserToReturn { get; set; }
        public bool UpdateResult { get; set; } = true;
        public User? UpdatedUser { get; private set; }

        public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Clone(UserToReturn));
        }

        public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            UpdatedUser = Clone(user);
            return Task.FromResult(UpdateResult);
        }

        private static User? Clone(User? user)
        {
            if (user is null)
            {
                return null;
            }

            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
