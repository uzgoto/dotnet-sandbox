﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.WinForms.Validators
{
    public enum InputType
    {
        Alpha,
        Numeric,
        AlphaNumeric,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ValidatorAttribute : Attribute
    {
        private static string Alpha = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public ValidatorAttribute(InputType inputType)
        {
            this.InputType = InputType;
        }
        public InputType InputType { get; private set; }

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

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    class ValidationTrrigerAttribute : ProxyAttribute
    {
        public ValidationTrrigerAttribute()
        {
        }

    }
}
