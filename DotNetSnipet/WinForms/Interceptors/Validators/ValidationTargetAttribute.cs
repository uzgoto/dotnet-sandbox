using System;
using System.Linq;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ValidationTargetAttribute : Attribute
    {
        private static readonly string Alpha = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public InputType InputType { get; private set; }
        public ValidationTargetAttribute(InputType inputType)
        {
            this.InputType = InputType;
        }

        public bool IsValid(string value)
        {
            switch (this.InputType)
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
    }
}
