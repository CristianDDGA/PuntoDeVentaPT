using Bogus;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using PuntoVenta.Domain.Enums;
using PuntoVenta.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoVenta.Infrastructure.Persistence;

public class DataSeedingService
{
    private readonly AppDbContext _context;

    public DataSeedingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task GenerarDatosEstresAsync()
    {
        // 5 minutos de tiempo de espera (timeout) para que Oracle procese todo con calma
        _context.Database.SetCommandTimeout(300);

        // Forzamos la cultura en inglés para los nombres de productos tecnológicos (ej. "Laptop Pro", "Gaming Mouse")
        var fakerTech = new Faker("en");
        var fakerEs = new Faker("es");

        // ==========================================
        // 1. GENERACIÓN DE DATOS EN MEMORIA (ARREGLOS)
        // ==========================================

        // --- Clientes (100,000) ---
        var customerIdSeed = 100;
        var clienteIds = new int[100000];
        var clienteDocumentos = new string[100000];
        var clienteNombres = new string[100000];
        var clienteApellidos = new string[100000];
        var clienteTelefonos = new string[100000];
        var clienteDirecciones = new string[100000];
        var clienteCiudades = new string[100000];
        var clienteEmails = new string[100000];
        // En Oracle mapeamos los booleanos como NUMBER(1) -> 1 para True
        var clienteEstados = Enumerable.Repeat((byte)1, 100000).ToArray();

        for (int i = 0; i < 100000; i++)
        {
            clienteIds[i] = ++customerIdSeed;
            clienteDocumentos[i] = fakerEs.Random.ReplaceNumbers("18########");
            clienteNombres[i] = fakerEs.Name.FirstName();
            clienteApellidos[i] = fakerEs.Name.LastName();
            clienteTelefonos[i] = fakerEs.Phone.PhoneNumber("09########");
            clienteDirecciones[i] = fakerEs.Address.StreetAddress();
            clienteCiudades[i] = "Ambato";
            clienteEmails[i] = fakerEs.Internet.Email(clienteNombres[i], clienteApellidos[i]);
        }

        // --- Productos Tecnológicos (100,000) ---
        var productIdSeed = 100;
        var productoIds = new int[100000];
        var productoNombres = new string[100000];
        var productoPrecios = new decimal[100000];
        var productoStocks = new int[100000];
        var productoEstados = Enumerable.Repeat((byte)1, 100000).ToArray();

        // Lista de prefijos y sufijos para darle un toque ultra tecnológico y realista
        var marcasTech = new[] { "Asus ROG", "MSI Pro", "Corsair", "Logitech G", "Razer", "Samsung Evo", "Kingston Fury", "Intel Core", "AMD Ryzen", "Sony", "Apple", "Dell UltraSharp", "Gigabyte" };
        var categoriasTech = new[] { "Gaming Laptop", "Mechanical Keyboard", "Wireless Mouse", "NVMe M.2 SSD", "Graphics Card RTX", "DDR5 RAM 16GB", "Curved Monitor", "Processor", "Liquid Cooling", "Headset 7.1" };

        for (int i = 0; i < 100000; i++)
        {
            productoIds[i] = ++productIdSeed;

            // Combina una marca real, una categoría y el generador de productos electrónicos de Bogus
            var marca = fakerTech.PickRandom(marcasTech);
            var categoria = fakerTech.PickRandom(categoriasTech);
            var modelo = fakerTech.Commerce.Product(); // Ej: "Computer", "Chips", etc.

            productoNombres[i] = $"{marca} {categoria} ({modelo})";

            // Los precios de tecnología son más elevados (ej: entre $15.00 por un cable hasta $1500.00 por una laptop)
            productoPrecios[i] = Math.Round(fakerTech.Random.Decimal(15.00m, 1499.00m), 2);
            productoStocks[i] = fakerTech.Random.Number(5, 300);
        }

        // --- Ventas (100,000) y Detalles Dinámicos ---
        var saleIdSeed = 100;
        var saleDetailIdSeed = 100;

        var ventaIds = new int[100000];
        var ventaClienteIds = new int[100000];
        var ventaFechas = new DateTime[100000];
        var ventaTiposPago = new byte[100000];
        var ventaSubtotales = new decimal[100000];
        var ventaImpuestos = new decimal[100000];
        var ventaTotales = new decimal[100000];
        var ventaEstados = new byte[100000];

        var detIds = new List<int>();
        var detVentaIds = new List<int>();
        var detProductoIds = new List<int>();
        var detCantidades = new List<int>();
        var detPreciosUnitarios = new List<decimal>();

        for (int i = 0; i < 100000; i++)
        {
            var actualSaleId = ++saleIdSeed;
            ventaIds[i] = actualSaleId;
            ventaClienteIds[i] = clienteIds[fakerEs.Random.Number(0, 99999)];
            ventaFechas[i] = fakerEs.Date.Past(1); // Ventas del último año
            ventaTiposPago[i] = 1; // 1: Solo efectivo (Cumpliendo el Requisito 30 del PDF)
            ventaEstados[i] = (byte)SaleStatus.Confirmed; // 1: Confirmed (Estado base en semilla para estrés)

            int itemsEnVenta = fakerEs.Random.Number(1, 2); // 1 o 2 artículos tecnológicos por factura
            decimal subtotalVenta = 0;

            for (int j = 0; j < itemsEnVenta; j++)
            {
                int randomProdIndex = fakerEs.Random.Number(0, 99999);
                int cantidad = fakerEs.Random.Number(1, 2); // Compras lógicas de tecnología (1 o 2 unidades)
                decimal precio = productoPrecios[randomProdIndex];

                subtotalVenta += (precio * cantidad);

                detIds.Add(++saleDetailIdSeed);
                detVentaIds.Add(actualSaleId);
                detProductoIds.Add(productoIds[randomProdIndex]);
                detCantidades.Add(cantidad);
                detPreciosUnitarios.Add(precio);
            }

            decimal IVA = Math.Round(subtotalVenta * 0.15m, 2); // Simulación de IVA 15% (Ecuador)
            ventaSubtotales[i] = Math.Round(subtotalVenta, 2);
            ventaImpuestos[i] = IVA;
            ventaTotales[i] = Math.Round(subtotalVenta + IVA, 2);
        }

        // ==========================================
        // 2. INSERCIÓN MASIVA EN ORACLE (ARRAY BINDING)
        // ==========================================
        var connectionString = _context.Database.GetConnectionString();

        using (var connection = new OracleConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 🧹 🚀 LIMPIEZA AUTOMÁTICA PREVIA (Garantiza entorno relacional limpio)
                    using (var limpiarCommand = connection.CreateCommand())
                    {
                        limpiarCommand.Transaction = transaction;
                        limpiarCommand.CommandText = @"
                            BEGIN
                                EXECUTE IMMEDIATE 'DELETE FROM ""StockMovements""';
                                EXECUTE IMMEDIATE 'DELETE FROM ""SaleDetails""';
                                EXECUTE IMMEDIATE 'DELETE FROM ""Sales""';
                                EXECUTE IMMEDIATE 'DELETE FROM ""Products""';
                                EXECUTE IMMEDIATE 'DELETE FROM ""Customers""';
                            END;";

                        await limpiarCommand.ExecuteNonQueryAsync();
                    }

                    // INSERCIÓN EN CUSTOMERS
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.ArrayBindCount = 100000;
                        command.CommandText = @"INSERT INTO ""Customers"" (""CustomerId"", ""DocumentNumber"", ""FirstName"", ""LastName"", ""Phone"", ""Address"", ""City"", ""Email"", ""IsActive"") VALUES (:CustomerId, :DocumentNumber, :FirstName, :LastName, :Phone, :Address, :City, :Email, :IsActive)";
                        command.Parameters.Add(new OracleParameter("CustomerId", OracleDbType.Int32) { Value = clienteIds });
                        command.Parameters.Add(new OracleParameter("DocumentNumber", OracleDbType.Varchar2) { Value = clienteDocumentos });
                        command.Parameters.Add(new OracleParameter("FirstName", OracleDbType.Varchar2) { Value = clienteNombres });
                        command.Parameters.Add(new OracleParameter("LastName", OracleDbType.Varchar2) { Value = clienteApellidos });
                        command.Parameters.Add(new OracleParameter("Phone", OracleDbType.Varchar2) { Value = clienteTelefonos });
                        command.Parameters.Add(new OracleParameter("Address", OracleDbType.Varchar2) { Value = clienteDirecciones });
                        command.Parameters.Add(new OracleParameter("City", OracleDbType.Varchar2) { Value = clienteCiudades });
                        command.Parameters.Add(new OracleParameter("Email", OracleDbType.Varchar2) { Value = clienteEmails });
                        command.Parameters.Add(new OracleParameter("IsActive", OracleDbType.Byte) { Value = clienteEstados });
                        await command.ExecuteNonQueryAsync();
                    }

                    // INSERCIÓN EN PRODUCTS
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.ArrayBindCount = 100000;
                        command.CommandText = @"INSERT INTO ""Products"" (""ProductId"", ""Name"", ""Price"", ""Stock"", ""IsActive"") VALUES (:ProductId, :Name, :Price, :Stock, :IsActive)";
                        command.Parameters.Add(new OracleParameter("ProductId", OracleDbType.Int32) { Value = productoIds });
                        command.Parameters.Add(new OracleParameter("Name", OracleDbType.Varchar2) { Value = productoNombres });
                        command.Parameters.Add(new OracleParameter("Price", OracleDbType.Decimal) { Value = productoPrecios });
                        command.Parameters.Add(new OracleParameter("Stock", OracleDbType.Int32) { Value = productoStocks });
                        command.Parameters.Add(new OracleParameter("IsActive", OracleDbType.Byte) { Value = productoEstados });
                        await command.ExecuteNonQueryAsync();
                    }

                    // INSERCIÓN EN SALES (CABECERA)
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.ArrayBindCount = 100000;
                        command.CommandText = @"INSERT INTO ""Sales"" (""SaleId"", ""CustomerId"", ""SaleDate"", ""PaymentType"", ""Subtotal"", ""TaxAmount"", ""Total"", ""Status"") VALUES (:SaleId, :CustomerId, :SaleDate, :PaymentType, :Subtotal, :TaxAmount, :Total, :Status)";
                        command.Parameters.Add(new OracleParameter("SaleId", OracleDbType.Int32) { Value = ventaIds });
                        command.Parameters.Add(new OracleParameter("CustomerId", OracleDbType.Int32) { Value = ventaClienteIds });
                        command.Parameters.Add(new OracleParameter("SaleDate", OracleDbType.TimeStamp) { Value = ventaFechas });
                        command.Parameters.Add(new OracleParameter("PaymentType", OracleDbType.Byte) { Value = ventaTiposPago });
                        command.Parameters.Add(new OracleParameter("Subtotal", OracleDbType.Decimal) { Value = ventaSubtotales });
                        command.Parameters.Add(new OracleParameter("TaxAmount", OracleDbType.Decimal) { Value = ventaImpuestos });
                        command.Parameters.Add(new OracleParameter("Total", OracleDbType.Decimal) { Value = ventaTotales });
                        command.Parameters.Add(new OracleParameter("Status", OracleDbType.Byte) { Value = ventaEstados });
                        await command.ExecuteNonQueryAsync();
                    }

                    // INSERCIÓN EN SALEDETAILS (DETALLE)
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.ArrayBindCount = detIds.Count;
                        command.CommandText = @"INSERT INTO ""SaleDetails"" (""SaleDetailId"", ""SaleId"", ""ProductId"", ""Quantity"", ""UnitPrice"") VALUES (:SaleDetailId, :SaleId, :ProductId, :Quantity, :UnitPrice)";
                        command.Parameters.Add(new OracleParameter("SaleDetailId", OracleDbType.Int32) { Value = detIds.ToArray() });
                        command.Parameters.Add(new OracleParameter("SaleId", OracleDbType.Int32) { Value = detVentaIds.ToArray() });
                        command.Parameters.Add(new OracleParameter("ProductId", OracleDbType.Int32) { Value = detProductoIds.ToArray() });
                        command.Parameters.Add(new OracleParameter("Quantity", OracleDbType.Int32) { Value = detCantidades.ToArray() });
                        command.Parameters.Add(new OracleParameter("UnitPrice", OracleDbType.Decimal) { Value = detPreciosUnitarios.ToArray() });
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}