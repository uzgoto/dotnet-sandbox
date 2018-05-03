using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Uzgoto.DotNetSnipet.Convertor
{
    public static class DataReaderExtension
    {
        public static IDataReader AsDataReader<T>(this IEnumerable<T> _self)
        {
            return EnumerableDataReader<T>.Create(_self) as IDataReader;
        }

        #region Nested class.
        internal class EnumerableDataReader<T> : IDataReader
        {
            private static readonly PropertyDescriptor[] _properties;

            static EnumerableDataReader()
            {
                _properties =
                    TypeDescriptor.GetProperties(typeof(T))
                        .OfType<PropertyDescriptor>()
                        .ToArray();
            }

            internal static IDataReader Create(IEnumerable<T> entities)
            {
                return new EnumerableDataReader<T>(entities);
            }

            private readonly IEnumerator<T> _entities;

            private EnumerableDataReader(IEnumerable<T> entities)
            {
                this._entities = entities.GetEnumerator();
            }

            #region IDisposable
            public void Dispose() { }
            #endregion

            #region IDataReader
            public void Close() => this.Dispose();
            public DataTable GetSchemaTable() => throw new NotImplementedException();
            public bool NextResult() => throw new NotImplementedException();
            public bool Read() => _entities.MoveNext();
            #endregion

            #region IDataRecord
            public int Depth => throw new NotImplementedException();
            public int FieldCount => _properties.Length;
            public bool IsClosed => throw new NotImplementedException();
            public int RecordsAffected => throw new NotImplementedException();

            public object this[int index] => this.GetValue(index);
            public object this[string name] => this.GetValue(this.GetOrdinal(name));

            public bool GetBoolean(int index) => throw new NotImplementedException();
            public byte GetByte(int index) => throw new NotImplementedException();
            public long GetBytes(int index, long fieldoffset, byte[] buffer, int bufferoffset, int lenght) => throw new NotImplementedException();
            public char GetChar(int index) => throw new NotImplementedException();
            public long GetChars(int index, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
            public IDataReader GetData(int index) => throw new NotImplementedException();
            public string GetDataTypeName(int index) => throw new NotImplementedException();
            public DateTime GetDateTime(int index) => throw new NotImplementedException();
            public decimal GetDecimal(int index) => throw new NotImplementedException();
            public double GetDouble(int index) => throw new NotImplementedException();
            public Type GetFieldType(int index) => _properties[index].PropertyType;
            public float GetFloat(int index) => throw new NotImplementedException();
            public Guid GetGuid(int index) => throw new NotImplementedException();
            public short GetInt16(int index) => throw new NotImplementedException();
            public int GetInt32(int index) => throw new NotImplementedException();
            public long GetInt64(int index) => throw new NotImplementedException();
            public string GetName(int index) => _properties[index].Name;
            public int GetOrdinal(string name) => Array.IndexOf(_properties, name);
            public string GetString(int index) => throw new NotImplementedException();
            public object GetValue(int index) => _properties[index].GetValue(_entities.Current);
            public int GetValues(object[] values) => throw new NotImplementedException();
            public bool IsDBNull(int index) => throw new NotImplementedException();
            #endregion
        }
        #endregion

        public static IEnumerable<T> AsEnumerable<T>(this IDataReader _self)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            if (_self.FieldCount != props.Count)
            {
                throw new InvalidOperationException();
            }

            while(_self.Read())
            {
                var entity = Activator.CreateInstance<T>();
                foreach (PropertyDescriptor prop in props)
                {
                    prop.SetValue(entity, _self[prop.Name]);
                }
                yield return entity;
            }
        }
    }
}
