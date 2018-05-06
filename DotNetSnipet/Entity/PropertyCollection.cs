﻿using System.Reflection;
using System.Linq;

namespace Uzgoto.DotNetSnipet.Entity
{
    internal static class PropertyCollection<TEntity>
    {
        internal static PropertyInfo[] Infos =>
            typeof(TEntity)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetCustomAttributes(typeof(EntityFieldAttribute)).Any())
                .ToArray();
    }
}