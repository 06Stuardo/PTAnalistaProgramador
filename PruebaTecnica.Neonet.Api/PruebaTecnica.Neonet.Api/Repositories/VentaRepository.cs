using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.Configuration;
using PruebaTecnica.Neonet.Api.DTOs.Venta;
using PruebaTecnica.Neonet.Api.Models;
using System.Data;

namespace PruebaTecnica.Neonet.Api.Repositories
{
    public class VentaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public VentaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Venta>> ObtenerTodosAsync()
        {
            var ventas = new List<Venta>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT id, fecha, clienteId, total
                FROM Ventas
                ORDER BY fecha DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ventas.Add(new Venta
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    clienteId = reader.GetInt32(reader.GetOrdinal("clienteId")),
                    total = reader.GetDecimal(reader.GetOrdinal("total"))
                });
            }

            return ventas;
        }

        public async Task<List<Venta>> ObtenerPorClienteAsync(int clienteId)
        {
            var ventas = new List<Venta>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT id, fecha, clienteId, total
                FROM Ventas
                WHERE clienteId = @clienteId
                ORDER BY fecha DESC";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@clienteId", clienteId);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ventas.Add(new Venta
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    clienteId =  reader.GetInt32(reader.GetOrdinal("clienteId")),
                    total =  reader.GetDecimal(reader.GetOrdinal("total"))
                });
            }

            return ventas;
        }

        public async Task<(int VentaId, decimal Total)> RegistrarVentaAsync(RegistrarVentaDto venta)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using SqlCommand command = new SqlCommand("SP_RegistrarVenta", connection );
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@clienteId", venta.clienteId );
            command.Parameters.AddWithValue("@fecha", venta.fecha);

            var detallesTable = new DataTable();
            detallesTable.Columns.Add("productoId", typeof(int));
            detallesTable.Columns.Add("cantidad",  typeof(int));
            detallesTable.Columns.Add("precioUnitario", typeof(decimal) );

            foreach (var detalle in venta.detalles)
            {
                detallesTable.Rows.Add(
                    detalle.idProducto,
                    detalle.cantidad,
                    detalle.precioUnitario
                );
            }

            var detallesParameter = command.Parameters.AddWithValue(
                "@detalles",
                detallesTable
            );

            detallesParameter.SqlDbType = SqlDbType.Structured;
            detallesParameter.TypeName = "TipoDetalleVenta";

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32(reader.GetOrdinal("VentaId")),
                    reader.GetDecimal(reader.GetOrdinal("Total"))
                );
            }

            throw new Exception("No fue posible registrar la venta.");
        }
    }
}
