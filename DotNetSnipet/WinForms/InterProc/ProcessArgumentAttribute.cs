using System;

namespace Uzgoto.DotNetSnipet.WinForms.InterProc
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ProcessArgumentAttribute : Attribute
    {
        public int Ordianl { get; private set; }
        public ProcessArgumentAttribute(int ordinal)
        {
            if (ordinal <= 0)
            {
                throw new ArgumentException($"{ordinal} is more than equals 1.");
            }
            this.Ordianl = ordinal;
        }
    }
}
