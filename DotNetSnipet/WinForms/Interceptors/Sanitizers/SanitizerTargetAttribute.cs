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
            this.InputType = inputType;
        }

        public bool IsValid(InputType inputType, string value)
        {
            switch (inputType)
            {
                case InputType.Alpha:
                    return value.All(c => Alpha.Contains(c));
                case InputType.Numeric:
                    return value.All(c => char.IsDigit(c));
                case InputType.AlphaNumeric:
                    return value.All(c => Alpha.Contains(c) || char.IsDigit(c));
                default:
                    throw new ArgumentException(nameof(value));
            }
        }
        public string Sanitize(string value) => (IsValid(this.InputType, value)) ? value : null;
    }
}
