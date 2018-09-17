using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace Uzgoto.DotNetSnipet.Office
{
    interface ISafeExcelMacroRunner : IDisposable
    {
        void OpenFile(string path);

        void Run(string bookName, string macroName);

        void Save(string bookName);
    }
}
