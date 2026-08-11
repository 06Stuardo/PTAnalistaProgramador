using Microsoft.Data.SqlClient;

namespace PruebaTecnica.Neonet.Api.Configuration
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("NeonetDb")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión NeonetDb."
                );
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
