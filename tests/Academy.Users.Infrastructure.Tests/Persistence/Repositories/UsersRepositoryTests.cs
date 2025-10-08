using Academy.Users.Domain.Users;
using Academy.Users.Infrastructure.Persistence;
using Academy.Users.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Academy.Users.Infrastructure.Tests.Persistence.Repositories;

public class UsersRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AcademyUsersDbContext _context;
    private readonly UsersRepository _repository;

    public UsersRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AcademyUsersDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new AcademyUsersDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new UsersRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        var user = CreateUser(1);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("First1", result!.FirstName);
        Assert.Equal("5511111111", result.PhoneNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        var user = CreateUser(2);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.FirstName = "Updated";
        user.PhoneNumber = "5522222222";

        var succeeded = await _repository.UpdateAsync(user, CancellationToken.None);

        Assert.True(succeeded);
        var refreshed = await _repository.GetByIdAsync(2, CancellationToken.None);
        Assert.NotNull(refreshed);
        Assert.Equal("Updated", refreshed!.FirstName);
        Assert.Equal("5522222222", refreshed.PhoneNumber);
    }

    private static User CreateUser(int id)
    {
        return new User
        {
            Id = id,
            FirstName = $"First{id}",
            LastName = $"Last{id}",
            Email = $"user{id}@example.com",
            PhoneNumber = "5511111111",
            Address = $"Address{id}",
            PasswordHash = $"Hash{id}",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
