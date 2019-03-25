using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace CapiControls.Data.Repositories
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration configuration;
        private readonly string connection;

        public BaseRepository(IConfiguration configuration, string connection)
        {
            this.configuration = configuration;
            this.connection = connection;
        }

        public IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(configuration.GetConnectionString(connection));
            }
        }
    }
}
