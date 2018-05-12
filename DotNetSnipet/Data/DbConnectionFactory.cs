using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Data
{
    public class DbConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory _provider;
        private readonly string _connectionString;
        private readonly string _name;

        public DbConnectionFactory(string connectionName)
        {
            if (connectionName == null) throw new ArgumentException(nameof(connectionName));

            var conStr = ConfigurationManager.ConnectionStrings[connectionName];
            if (conStr == null) throw new ConfigurationErrorsException(
                 $"Failed to find connection string named '{nameof(connectionName)}' in app.config.");

            this._name = connectionName;
            this._connectionString = conStr.ConnectionString;
            this._provider = DbProviderFactories.GetFactory(conStr.ProviderName);
        }

        public IDbConnection Create()
        {
            var connection = this._provider.CreateConnection();
            if (connection == null) throw new ConfigurationErrorsException(
                 $"Failed to create a connection using the connection string named '{nameof(this._name)}' in app.config.");

            connection.ConnectionString = this._connectionString;
            connection.Open();
            return connection;
        }
    }
}
