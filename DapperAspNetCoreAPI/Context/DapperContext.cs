using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperAspNetCoreAPI.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration, string connectionString)
        {
            _configuration = configuration;
            _connectionString = connectionString;   
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
