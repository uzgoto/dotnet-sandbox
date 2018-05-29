using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uzgoto.DotNetSnipet.WinForms.InterProc
{
    public sealed class Helper<TArgument> where TArgument : struct
    {
        private static PropertyInfo[] Properties =>
            typeof(TArgument)
                .GetProperties()
                .Where(prop => prop.GetCustomAttributes<ProcessArgumentAttribute>().Any())
                .OrderBy(prop => prop.GetCustomAttribute<ProcessArgumentAttribute>().Ordianl)
                .ToArray();

        public static TArgument Create(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentException("Argument is null.", nameof(args));
            }
            if (args.Length != Properties.Length)
            {
                throw new ArgumentException($"Argument length is not match.", nameof(args));
            }

            var argument = new TArgument();
            foreach(var anon in Properties.Zip(args, (prop, value) => new { Prop = prop, Value = value }))
            {
                anon.Prop.SetValue(argument, anon.Value);
            }
            return argument;
        }

        public static IEnumerable<string> ConvertToProcessArgs(TArgument argument)
        {
            return
                Properties.Select(prop => Convert.ToString(prop.GetValue(argument)));
        }
    }
}
