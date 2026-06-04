using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoVenta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AddColumn<int>(
                name: "NewStock",
                table: "StockMovements",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousStock",
                table: "StockMovements",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);
            */

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "SaleDetails",
                type: "NVARCHAR2(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "Histórico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewStock",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "PreviousStock",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "SaleDetails");
        }
    }
}
