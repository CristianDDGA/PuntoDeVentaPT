using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> saleBuilder)
    {
        saleBuilder.ToTable("Sales");

        saleBuilder.HasKey(sale => sale.SaleId);

        saleBuilder.Property(sale => sale.SaleDate)
            .IsRequired();

        saleBuilder.Property(sale => sale.PaymentType)
            .IsRequired();

        saleBuilder.Property(sale => sale.Status)
            .IsRequired();

        saleBuilder.Property(sale => sale.Subtotal)
            .HasColumnType("decimal(10,2)");

        saleBuilder.Property(sale => sale.TaxAmount)
            .HasColumnType("decimal(10,2)");

        saleBuilder.Property(sale => sale.Total)
            .HasColumnType("decimal(10,2)");

        saleBuilder.HasOne(sale => sale.Customer)
            .WithMany()
            .HasForeignKey(sale => sale.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        saleBuilder.HasMany(sale => sale.Details)
            .WithOne()
            .HasForeignKey(saleDetail => saleDetail.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}