using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.WinForms.Messages
{
    interface IMessage
    {
        string Code { get; }
        string Text { get; }
        Level Level { get; }
    }
}
