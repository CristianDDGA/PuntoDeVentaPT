using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PuntoVenta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SemillaClientesPrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "City", "DocumentNumber", "Email", "FirstName", "LastName", "Phone" },
                values: new object[,]
                {
                    { 1, "Av. Cevallos y Espejo", "Ambato", "1850123456", "juan.perez@gmail.com", "Juan", "Pérez", "0999999991" },
                    { 2, "Av. de los Shyris", "Quito", "1720123456", "maria.lopez@gmail.com", "María", "López", "0999999992" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2);
        }
    }
}
