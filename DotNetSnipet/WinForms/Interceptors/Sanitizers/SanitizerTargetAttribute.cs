using System;
using System.Linq;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SanitizerTargetAttribute : Attribute
    {
        private static readonly string Alpha = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public InputType InputType { get; private set; }
        public SanitizerTargetAttribute(InputType inputType)
        {
            this.InputType = InputType;
        }

        public string Sanitize(string value)
        {
            switch (this.InputType)
            {
                case InputType.Alpha:
                    return (value.All(c => Alpha.Contains(c))) ? value : null;
                case InputType.Numeric:
                    return (value.All(c => char.IsDigit(c))) ? value : null;
                case InputType.AlphaNumeric:
                    return (value.All(c => Alpha.Contains(c) || char.IsDigit(c))) ? value : null;
                default:
                    throw new ArgumentException(nameof(value));
            }
        }
    }
}
