using System;
using System.Linq;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors.Sanitizers
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
                    return true;
            }
        }
        public string Sanitize(string value) => (IsValid(this.InputType, value)) ? value : null;
    }

    [Flags]
    public enum InputType
    {
        Alpha =   0x0001,
        Numeric = 0x0010,
        Symbol =  0x0100,
        
        AlphaNumeric = Alpha | Numeric,
        AlphaSymbol = Alpha | Symbol,
        NumericSymbol = Numeric | Symbol,
        AlphaNumericSymbol = AlphaNumeric | Symbol,

        FileName = 0x1000,
    }
}
