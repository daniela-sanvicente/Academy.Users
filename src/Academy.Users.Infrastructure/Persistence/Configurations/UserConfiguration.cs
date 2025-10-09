using Academy.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Users.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.FirstName).IsRequired();
        builder.Property(user => user.LastName).IsRequired();
        builder.Property(user => user.Email).IsRequired();
        builder.Property(user => user.PhoneNumber).IsRequired();
        builder.Property(user => user.Address).IsRequired();
        builder.Property(user => user.Password).IsRequired();
        builder.Property(user => user.Status).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
        builder.Property(user => user.UpdatedAt).IsRequired();
    }
}
