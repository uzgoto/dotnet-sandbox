using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Forms.WinForms.Interceptors.Sanitizers
{
    public abstract class SanitizeBaseAttribute : Attribute
    {
        public abstract InputType InputType { get; }

        public abstract bool IsValid(string value);
        public abstract string Sanitize(string value);
    }
}
