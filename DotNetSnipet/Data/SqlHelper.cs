using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Uzgoto.DotNetSnipet.Data
{
    public class SqlHelper : IDisposable
    {
        private SqlConnection _connection;
        private SqlHelper(SqlConnection connection)
        {
            if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                throw new ArgumentException("Connection string is empty.");
            }
            this._connection = connection;
        }

        public static SqlHelper Open(SqlConnection connection)
        {
            var command = new SqlHelper(connection);
            command._connection.Open();
            return command;
        }

        public IDataReader ExecuteReader(string query)
        {
            using (var command = this._connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                return command.ExecuteReader();
            }
        }

        public int ExecuteNonQuery(string query)
        {
            using (var command = this._connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                return command.ExecuteNonQuery();
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return this._connection.BeginTransaction();
        }

        public void ExecuteBulkCopy(IDataReader reader, string tableName)
        {
            using (var command = this._connection.CreateCommand())
            {
                using (var sbc = new SqlBulkCopy(this._connection))
                {
                    sbc.BatchSize = 1000;
                    sbc.DestinationTableName = tableName;
                    sbc.WriteToServer(reader);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._connection.Close();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~SqlConnection() {
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
