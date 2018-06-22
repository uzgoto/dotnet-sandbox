using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Extensions
{
    internal static class WinFormsExtensions
    {
        public static IEnumerable<(Control, T)> EnumerateControlsWith<T>(this Form form) where T : Attribute
        {
            if (form.Controls == null) throw new ArgumentException("This form has no controls.");

            var controls = form.Controls.OfType<Control>();

            var fields = form.GetType().GetRuntimeFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(form);
                if (value is Control control)
                {
                    var attribute = field.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        yield return (control, attribute);
                    }
                }
            }
        }
    }
}
