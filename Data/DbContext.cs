using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;

namespace Uzgoto.DotNetSnipet.Data
{
    public sealed class DbContext : IDbContext
    {
        private readonly IDbConnection _connection;

        public static DbContext Create()
        {
            const string CONNECTION_NAME = "ConnectionName";
            var connectionName =
                ConfigurationManager.AppSettings[CONNECTION_NAME]
                ?? throw new ConfigurationErrorsException($"Failed to find settings named '{CONNECTION_NAME}'.");
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ConfigurationErrorsException($"Value of {CONNECTION_NAME} is Empty.");
            }
            return new DbContext(connectionName);
        }

        private DbContext(string connectionName)
        {
            if (connectionName == null) throw new ArgumentException(nameof(connectionName));

            var conStr =
                ConfigurationManager.ConnectionStrings[connectionName]
                ?? throw new ConfigurationErrorsException($"Failed to find connection string named '{nameof(connectionName)}'.");

            this._connection =
                DbProviderFactories.GetFactory(conStr.ProviderName).CreateConnection()
                ?? throw new ConfigurationErrorsException($"Failed to create a connection using the connection string named '{nameof(connectionName)}'.");

            this._connection.ConnectionString = conStr.ConnectionString;
        }

        public void OpenIfClosed()
        {
            if (this._connection.State == ConnectionState.Closed || this._connection.State == ConnectionState.Broken)
            {
                this._connection.Open();
            }
        }

        #region Implement IDbContext
        public IDbTransaction BeginTransaction()
        {
            this.OpenIfClosed();
            return this._connection.BeginTransaction();
        }

        public void Commit(IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentException($"{nameof(transaction)} is null.");
            }
            if (!transaction.Connection.Equals(this._connection))
            {
                throw new ArgumentException($"Connection of {nameof(transaction)} is not supported.");
            }
            if (transaction.Connection.State == ConnectionState.Broken || transaction.Connection.State == ConnectionState.Closed)
            {
                throw new ArgumentException($"Connection of {nameof(transaction)} is closed.");
            }

            transaction.Commit();
            this._connection.Close();
        }

        public void RollBack(IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentException($"{nameof(transaction)} is null.");
            }
            if (!transaction.Connection.Equals(this._connection))
            {
                throw new ArgumentException($"Connection of {nameof(transaction)} is not supported.");
            }
            if (transaction.Connection.State == ConnectionState.Broken || transaction.Connection.State == ConnectionState.Closed)
            {
                throw new ArgumentException($"Connection of {nameof(transaction)} is closed.");
            }

            transaction.Rollback();
            this._connection.Close();
        }

        public void ExecuteBulkCopy<TEntity>(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBulkCopy(IEnumerable<dynamic> entities)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBulkCopy(IDataReader dataReader)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBulkCopy(Stream stream)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string commandText, params IDataParameter[] sqlParameters)
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(string commandText, params IDataParameter[] sqlParameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> ExecuteReaderAsEnumerable<TEntity>(string commandText, params IDataParameter[] sqlParameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> ExecuteReaderAsEnumerable(string commandText, params IDataParameter[] sqlParameters)
        {
            throw new NotImplementedException();
        }

        public TResult ExecuteScalar<TResult>(string commandText, params IDataParameter[] sqlParameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._connection?.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~DbContext() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
