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
    public class DbConnection
    {
        public IDbConnection Connection { get; private set; }

        public DbConnection(string connectionName)
        {
            if (connectionName == null) throw new ArgumentException(nameof(connectionName));

            var conStr =
                ConfigurationManager.ConnectionStrings[connectionName]
                ?? throw new ConfigurationErrorsException($"Failed to find connection string named '{nameof(connectionName)}'.");

            this.Connection =
                DbProviderFactories.GetFactory(conStr.ProviderName).CreateConnection()
                ?? throw new ConfigurationErrorsException($"Failed to create a connection using the connection string named '{nameof(connectionName)}'.");

            this.Connection.ConnectionString = conStr.ConnectionString;
            this.Connection.Open();
        }
    }
}
