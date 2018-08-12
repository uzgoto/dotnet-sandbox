using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Forms.WinForms.Interceptors.Sanitizers
{
    [Flags]
    public enum InputType
    {
        Alpha = 0x0001,
        Numeric = 0x0010,
        Symbol = 0x0100,

        AlphaNumeric = Alpha | Numeric,
        AlphaSymbol = Alpha | Symbol,
        NumericSymbol = Numeric | Symbol,
        AlphaNumericSymbol = AlphaNumeric | Symbol,

        FileName = 0x1000,
    }
}
