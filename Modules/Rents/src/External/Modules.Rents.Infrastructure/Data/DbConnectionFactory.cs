using System.Data.Common;
using Modules.Rents.Application.Abstractions;
using Npgsql;

namespace Modules.Rents.Infrastructure.Data
{
    public class DbConnectionFactory(string ConnectionString) : IDbConnectionFactory
    {
        public async Task<DbConnection> CreateSqlConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}