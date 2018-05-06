using System.Reflection;
using System.Linq;
using System;
using System.Globalization;

namespace Uzgoto.DotNetSnipet.Entity
{
    internal static class PropertyCollection<TEntity>
    {
        internal static Property[] Infos =>
            typeof(TEntity)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetCustomAttributes(typeof(EntityFieldAttribute)).Any())
                .Select(prop => new Property(prop))
                .ToArray();

        internal static int Ordinal(string name) => Array.FindIndex(Infos, info => info.Name == name);
        internal static int MappedOrdinal(string mappedName) => Array.FindIndex(Infos, info => info.MappedName == mappedName);

        internal class Property
        {
            private PropertyInfo _info;
            internal Property(PropertyInfo info)
            {
                this._info = info;
            }
            public Type PropertyType => this._info.PropertyType;
            public string Name => this._info.Name;
            public string MappedName => ((EntityFieldAttribute)this._info.GetCustomAttribute(typeof(EntityFieldAttribute))).MappingTo;
            public object GetValue(object obj) => this._info.GetValue(obj);
            public void SetValue(object obj, object value) => this._info.SetValue(obj, value);
        }
    }
}
