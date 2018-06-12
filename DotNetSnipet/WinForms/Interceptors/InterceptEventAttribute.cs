using System;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InterceptEventAttribute : Attribute
    {
        public string EventName { get; private set; }
        public InterceptEventAttribute(string eventName)
        {
            this.EventName = eventName;
        }
    }
}
