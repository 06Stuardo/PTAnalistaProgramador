using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.Configuration;
using PruebaTecnica.Neonet.Api.DTOs.Cliente;
using PruebaTecnica.Neonet.Api.Models;

namespace PruebaTecnica.Neonet.Api.Repositories
{
    public class ClienteRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ClienteRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            var clientes = new List<Cliente>();
            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT id, nombre, email
                FROM Clientes";

            using SqlCommand command = new SqlCommand(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                clientes.Add(new Cliente
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    
                });
            }

            return clientes;

        }

        public async Task<int> RegistrarAsync(RegistrarClienteDto cliente)
        {
            using SqlConnection connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string query = @"
                                    INSERT INTO Clientes(nombre, email)
                                    OUTPUT INSERTED.id
                                    VALUES (@nombre, @email)
                                 ";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@nombre", cliente.nombre);
            command.Parameters.AddWithValue("@email", cliente.email);

            var resultado = await command.ExecuteScalarAsync();

            return Convert.ToInt32(resultado);
        }

    }
}
