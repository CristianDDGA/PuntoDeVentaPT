using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> productBuilder)
    {
        productBuilder.ToTable("Products");

        productBuilder.HasKey(product => product.ProductId);

        productBuilder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(150);

        productBuilder.Property(product => product.Price)
            .HasColumnType("decimal(10,2)");

        productBuilder.Property(product => product.Stock)
            .IsRequired();
    }
}