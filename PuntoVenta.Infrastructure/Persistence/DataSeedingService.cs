using Bogus;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using PuntoVenta.Infrastructure.Persistence;

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

        var faker = new Faker("es");

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

        for (int i = 0; i < 100000; i++)
        {
            clienteIds[i] = ++customerIdSeed;
            clienteDocumentos[i] = faker.Random.ReplaceNumbers("18########");
            clienteNombres[i] = faker.Name.FirstName();
            clienteApellidos[i] = faker.Name.LastName();
            clienteTelefonos[i] = faker.Phone.PhoneNumber("09########");
            clienteDirecciones[i] = faker.Address.StreetAddress();
            clienteCiudades[i] = "Ambato";
            clienteEmails[i] = faker.Internet.Email(clienteNombres[i], clienteApellidos[i]);
        }

        // --- Productos (100,000) ---
        var productIdSeed = 100;
        var productoIds = new int[100000];
        var productoNombres = new string[100000];
        var productoPrecios = new decimal[100000];
        var productoStocks = new int[100000];

        for (int i = 0; i < 100000; i++)
        {
            productoIds[i] = ++productIdSeed;
            productoNombres[i] = faker.Commerce.ProductName();
            productoPrecios[i] = Math.Round(faker.Random.Decimal(0.50m, 40.00m), 2);
            productoStocks[i] = faker.Random.Number(5, 300);
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
            ventaClienteIds[i] = clienteIds[faker.Random.Number(0, 99999)];
            ventaFechas[i] = faker.Date.Past(1); // Ventas registradas en el último año
            ventaTiposPago[i] = (byte)faker.Random.Number(1, 3); // 1: Efectivo, 2: Tarjeta, 3: Transferencia
            ventaEstados[i] = 1; // 1: Completada

            int itemsEnVenta = faker.Random.Number(1, 2); // 1 o 2 productos por venta
            decimal subtotalVenta = 0;

            for (int j = 0; j < itemsEnVenta; j++)
            {
                int randomProdIndex = faker.Random.Number(0, 99999);
                int cantidad = faker.Random.Number(1, 5);
                decimal precio = productoPrecios[randomProdIndex];

                subtotalVenta += (precio * cantidad);

                detIds.Add(++saleDetailIdSeed);
                detVentaIds.Add(actualSaleId);
                detProductoIds.Add(productoIds[randomProdIndex]);
                detCantidades.Add(cantidad);
                detPreciosUnitarios.Add(precio);
            }

            decimal IVA = Math.Round(subtotalVenta * 0.15m, 2); // Simulación de IVA 15%
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
                    // 🧹 🚀 LIMPIEZA AUTOMÁTICA PREVIA
                    // Borramos en orden estricto de dependencias para no violar las Foreign Keys
                    using (var limpiarCommand = connection.CreateCommand())
                    {
                        limpiarCommand.Transaction = transaction;
                        limpiarCommand.CommandText = @"
                            BEGIN
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
                        command.CommandText = @"INSERT INTO ""Customers"" (""CustomerId"", ""DocumentNumber"", ""FirstName"", ""LastName"", ""Phone"", ""Address"", ""City"", ""Email"") VALUES (:CustomerId, :DocumentNumber, :FirstName, :LastName, :Phone, :Address, :City, :Email)";
                        command.Parameters.Add(new OracleParameter("CustomerId", OracleDbType.Int32) { Value = clienteIds });
                        command.Parameters.Add(new OracleParameter("DocumentNumber", OracleDbType.Varchar2) { Value = clienteDocumentos });
                        command.Parameters.Add(new OracleParameter("FirstName", OracleDbType.Varchar2) { Value = clienteNombres });
                        command.Parameters.Add(new OracleParameter("LastName", OracleDbType.Varchar2) { Value = clienteApellidos });
                        command.Parameters.Add(new OracleParameter("Phone", OracleDbType.Varchar2) { Value = clienteTelefonos });
                        command.Parameters.Add(new OracleParameter("Address", OracleDbType.Varchar2) { Value = clienteDirecciones });
                        command.Parameters.Add(new OracleParameter("City", OracleDbType.Varchar2) { Value = clienteCiudades });
                        command.Parameters.Add(new OracleParameter("Email", OracleDbType.Varchar2) { Value = clienteEmails });
                        await command.ExecuteNonQueryAsync();
                    }

                    // INSERCIÓN EN PRODUCTS
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.ArrayBindCount = 100000;
                        command.CommandText = @"INSERT INTO ""Products"" (""ProductId"", ""Name"", ""Price"", ""Stock"") VALUES (:ProductId, :Name, :Price, :Stock)";
                        command.Parameters.Add(new OracleParameter("ProductId", OracleDbType.Int32) { Value = productoIds });
                        command.Parameters.Add(new OracleParameter("Name", OracleDbType.Varchar2) { Value = productoNombres });
                        command.Parameters.Add(new OracleParameter("Price", OracleDbType.Decimal) { Value = productoPrecios });
                        command.Parameters.Add(new OracleParameter("Stock", OracleDbType.Int32) { Value = productoStocks });
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