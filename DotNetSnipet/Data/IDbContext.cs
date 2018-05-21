using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Data
{
    interface IDbContext
    {
        IDataReader ExecuteReader(string commandText, params IDataParameter[] sqlParameters);
        IEnumerable<TEntity> ExecuteReaderAsEnumerable<TEntity>(string commandText, params IDataParameter[] sqlParameters);
        IEnumerable<dynamic> ExecuteReaderAsEnumerable(string commandText, params IDataParameter[] sqlParameters);
        int ExecuteNonQuery(string commandText, params IDataParameter[] sqlParameters);
        TResult ExecuteScalar<TResult>(string commandText, params IDataParameter[] sqlParameters);
        void ExecuteBulkCopy<TEntity>(IEnumerable<TEntity> entities);
        void ExecuteBulkCopy(IEnumerable<dynamic> entities);
        void ExecuteBulkCopy(IDataReader dataReader);
        void ExecuteBulkCopy(Stream stream);
        IDbTransaction BeginTransaction();
        void Commit();
        void RollBack();
    }
}
