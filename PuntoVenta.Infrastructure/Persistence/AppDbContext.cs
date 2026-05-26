using Microsoft.EntityFrameworkCore;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence.Configurations;

namespace PuntoVenta.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer>   Customers   => Set<Customer>();
    public DbSet<Product>    Products    => Set<Product>();
    public DbSet<Sale>       Sales       => Set<Sale>();
    public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleDetailConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}