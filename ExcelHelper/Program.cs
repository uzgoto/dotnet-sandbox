using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Uzgoto.DotNetSnipet.Office
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var runner = new SafeExcelMacroRunner())
            {
                var pathToFiles =
                    Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Desktop", "work");

                var dataBookName = "test.csv";
                var macroBookName = "test.xlsm";
                runner.OpenFile(Path.Combine(pathToFiles, dataBookName));
                runner.OpenFile(Path.Combine(pathToFiles, macroBookName));
                runner.Run(macroBookName, "Macro1");
                runner.Save(macroBookName);
            }
        }
    }
}
