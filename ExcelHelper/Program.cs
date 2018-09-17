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
            var pathToFiles =
                Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Desktop", "work");

            var templatePath = Path.Combine(pathToFiles, "template.xlsm");

            Parallel.For(1, 5, idx =>
            {
                var macroBookName = $"test{idx}.xlsm";
                var macroPath = Path.Combine(pathToFiles, macroBookName);
                var dataPath = Path.Combine(pathToFiles, $"test.csv");

                if (File.Exists(macroPath))
                {
                    File.Delete(macroPath);
                }

                File.Copy(templatePath, macroPath);

                using (var runner = new SafeExcelMacroRunner())
                {
                    runner.OpenFile(dataPath);
                    runner.OpenFile(macroPath);
                    runner.Run(macroBookName, "Macro1");
                    runner.Save(macroBookName);
                }
            });
        }
    }
}
