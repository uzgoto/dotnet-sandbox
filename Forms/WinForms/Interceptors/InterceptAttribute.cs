using System;

namespace Uzgoto.DotNetSnipet.Forms.WinForms.Interceptors
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class InterceptAttribute : Attribute
    {
        public string EventName { get; private set; }
        public InterceptAttribute(string eventName)
        {
            this.EventName = eventName;
        }
    }
}
