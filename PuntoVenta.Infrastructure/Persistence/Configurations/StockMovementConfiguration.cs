using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> stockMovementBuilder)
    {
        stockMovementBuilder.ToTable("StockMovements");

        stockMovementBuilder.HasKey(stockMovement => stockMovement.StockMovementId);

        stockMovementBuilder.Property(stockMovement => stockMovement.Quantity)
            .IsRequired();

        stockMovementBuilder.Property(stockMovement => stockMovement.MovementType)
            .IsRequired();

        stockMovementBuilder.Property(stockMovement => stockMovement.Reference)
            .HasMaxLength(200);

        stockMovementBuilder.Property(stockMovement => stockMovement.CreatedAt)
            .IsRequired();

        stockMovementBuilder.HasOne(stockMovement => stockMovement.Product)
            .WithMany()
            .HasForeignKey(stockMovement => stockMovement.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        stockMovementBuilder.HasOne(stockMovement => stockMovement.User)
            .WithMany()
            .HasForeignKey(stockMovement => stockMovement.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}