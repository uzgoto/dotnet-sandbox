using System;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public interface IInterceptor<T>
    {
        void Intercept(T instance, EventHandler preInvoke, EventHandler postInvoke);
    }
}
