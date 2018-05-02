using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Linq;

namespace Uzgoto.DotNetSnipet.Convertor
{
    public static class DataTableExtension
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> _self)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in props)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                table.Columns.Add(prop.Name, type);
            }

            foreach (var data in _self)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in props)
                {
                    var value = prop.GetValue(data) ?? DBNull.Value;

                    row[prop.Name] = value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IEnumerable<T> AsEnumerable<T>(this DataTable _self) where T : class
        {
            var props = TypeDescriptor.GetProperties(typeof(T));

            foreach (DataRow row in _self.Rows)
            {
                var data = Activator.CreateInstance<T>();
                foreach (PropertyDescriptor prop in props)
                {
                    prop.SetValue(data, row[prop.Name]);
                }
                yield return data;
            }
        }
    }
}
