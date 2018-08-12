using System;
using System.Linq;

namespace Uzgoto.DotNetSnipet.Forms.WinForms.Interceptors.Sanitizers
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class SanitizeAttribute : SanitizeBaseAttribute
    {
        public override InputType InputType { get; }
        public SanitizeAttribute(InputType inputType)
        {
            this.InputType = inputType;
        }

        public override bool IsValid(string value)
        {
            // It is not valid to convert non-numeric charactor to numeric type.
            if (this.InputType == InputType.Numeric)
            {
                return value.All(c => char.IsDigit(c));
            }

            return true;
        }

        public override string Sanitize(string value) =>
            (IsValid(value)) ? value : null;
    }
}
