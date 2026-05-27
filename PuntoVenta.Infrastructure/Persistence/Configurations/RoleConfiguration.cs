using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> roleBuilder)
    {
        roleBuilder.ToTable("Roles");

        roleBuilder.HasKey(role => role.RoleId);

        roleBuilder.HasIndex(role => role.Name)
            .IsUnique();

        roleBuilder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(100);

        roleBuilder.Property(role => role.IsActive)
            .IsRequired();
    }
}