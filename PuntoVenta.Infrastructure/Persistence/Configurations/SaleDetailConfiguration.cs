using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
{
    public void Configure(EntityTypeBuilder<SaleDetail> saleDetailBuilder)
    {
        saleDetailBuilder.ToTable("SaleDetails");

        saleDetailBuilder.HasKey(saleDetail => saleDetail.SaleDetailId);

        saleDetailBuilder.Property(saleDetail => saleDetail.Quantity)
            .IsRequired();

        saleDetailBuilder.Property(saleDetail => saleDetail.UnitPrice)
            .HasColumnType("decimal(10,2)");

        // Subtotal es calculado en memoria, no se persiste (ya lo maneja SQL)
        saleDetailBuilder.Ignore(saleDetail => saleDetail.Subtotal);

        saleDetailBuilder.HasOne(saleDetail => saleDetail.Product)
            .WithMany()
            .HasForeignKey(saleDetail => saleDetail.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}