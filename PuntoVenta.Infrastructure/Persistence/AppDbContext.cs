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
    public DbSet<Role>       Roles       => Set<Role>();
    public DbSet<User>       Users       => Set<User>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<ErrorLog>   ErrorLogs   => Set<ErrorLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new SaleConfiguration());
        modelBuilder.ApplyConfiguration(new SaleDetailConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
        modelBuilder.ApplyConfiguration(new ErrorLogConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}