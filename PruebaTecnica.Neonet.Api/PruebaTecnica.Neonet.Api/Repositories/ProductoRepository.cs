using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.Configuration;
using PruebaTecnica.Neonet.Api.Models;

namespace PruebaTecnica.Neonet.Api.Repositories
{
    public class ProductoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ProductoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            var productos = new List<Producto>();

            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT id, nombre, precio, stock
                FROM Productos";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                    stock = reader.GetInt32(reader.GetOrdinal("stock"))
                });
            }

            return productos;
        }
    }
}
