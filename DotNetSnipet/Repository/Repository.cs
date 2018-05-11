using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Repository
{
    public abstract class Repository<TEntity>
    {
        protected IEnumerable<TEntity> EnumerateDataReader(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return CreateEntity(reader);
                }
            }
        }

        private TEntity CreateEntity(IDataRecord record)
        {
            var entityProps = typeof(TEntity).GetProperties();
            if (record.FieldCount != entityProps.Length)
            {
                throw new IndexOutOfRangeException(
                    $"Field count is not match. {nameof(IDataRecord)} is {record.FieldCount} <-> {nameof(TEntity)} is {entityProps.Length}");
            }

            var entity = Activator.CreateInstance<TEntity>();
            foreach (var prop in entityProps)
            {
                prop.SetValue(entity, record[prop.Name]);
            }
            return entity;
        }
    }
}
