using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public interface IEventInterceptor<T> where T : IComponent
    {
        void Intercept(EventHandler preHandler, EventHandler postHandler);
    }
}
