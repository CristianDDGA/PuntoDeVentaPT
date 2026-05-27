using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> userBuilder)
    {
        userBuilder.ToTable("Users");

        userBuilder.HasKey(user => user.UserId);

        userBuilder.Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(100);

        userBuilder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        userBuilder.Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(150);

        userBuilder.Property(user => user.Email)
            .HasMaxLength(150);

        userBuilder.Property(user => user.IsActive)
            .IsRequired();

        userBuilder.HasIndex(user => user.Username)
            .IsUnique();

        userBuilder.HasOne(user => user.Role)
            .WithMany()
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}