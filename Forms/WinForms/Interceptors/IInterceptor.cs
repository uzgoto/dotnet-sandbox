using System;

namespace Uzgoto.DotNetSnipet.Forms.WinForms.Interceptors
{
    public interface IInterceptor<T>
    {
        void Intercept(T instance, EventHandler preInvoke, EventHandler postInvoke);
    }
}
