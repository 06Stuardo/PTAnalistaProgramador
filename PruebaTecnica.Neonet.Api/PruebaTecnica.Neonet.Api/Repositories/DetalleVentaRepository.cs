using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.Configuration;
using PruebaTecnica.Neonet.Api.Models;

namespace PruebaTecnica.Neonet.Api.Repositories
{
    public class DetalleVentaRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public DetalleVentaRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<DetalleVenta>> ObtenerPorVentaAsync(int ventaId)
        {
            var detalles = new List<DetalleVenta>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT id, ventaId, productoId, cantidad, precioUnitario
                FROM DetalleVenta
                WHERE ventaId = @ventaId";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ventaId", ventaId);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                detalles.Add(new DetalleVenta
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    ventaId = reader.GetInt32(reader.GetOrdinal("ventaId")),
                    productoId = reader.GetInt32(reader.GetOrdinal("productoId")),
                    cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    precioUnitario = reader.GetDecimal(reader.GetOrdinal("precioUnitario"))
                });
            }

            return detalles;
        }
    }
}
