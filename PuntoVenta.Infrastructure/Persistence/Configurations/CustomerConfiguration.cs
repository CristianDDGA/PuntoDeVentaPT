using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> customerBuilder)
    {
        customerBuilder.ToTable("Customers");

        customerBuilder.HasKey(customer => customer.CustomerId);

        customerBuilder.Property(customer => customer.DocumentNumber)
            .IsRequired()
            .HasMaxLength(20);

        customerBuilder.Property(customer => customer.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        customerBuilder.Property(customer => customer.LastName)
            .IsRequired()
            .HasMaxLength(100);

        customerBuilder.Property(customer => customer.Phone)
            .HasMaxLength(20);

        customerBuilder.Property(customer => customer.Address)
            .HasMaxLength(200);

        customerBuilder.Property(customer => customer.City)
            .HasMaxLength(100);

        customerBuilder.Property(customer => customer.Email)
            .HasMaxLength(150);

        // Index to optimise LIKE searches by last name in the customer search modal.
        customerBuilder.HasIndex(customer => customer.LastName)
            .HasDatabaseName("inx_apellido");

        customerBuilder.HasIndex(customer => customer.DocumentNumber)
            .HasDatabaseName("inx_documento");
        // Quitamos la palabra "Customer" después de cada "new"
        customerBuilder.HasData(
            new
            {
                CustomerId = 1,
                DocumentNumber = "1850123456",
                FirstName = "Juan",
                LastName = "Pérez",
                Phone = "0999999991",
                Address = "Av. Cevallos y Espejo",
                City = "Ambato",
                Email = "juan.perez@gmail.com"
            },
            new
            {
                CustomerId = 2,
                DocumentNumber = "1720123456",
                FirstName = "María",
                LastName = "López",
                Phone = "0999999992",
                Address = "Av. de los Shyris",
                City = "Quito",
                Email = "maria.lopez@gmail.com"
            }
        );
    }
}