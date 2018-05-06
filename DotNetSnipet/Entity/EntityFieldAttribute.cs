using System;
using System.Collections.Generic;
using System.Text;

namespace Uzgoto.DotNetSnipet.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EntityFieldAttribute : Attribute
    {
        public string MappingTo { get; set; }
    }
}
