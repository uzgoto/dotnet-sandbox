using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace Uzgoto.DotNetSnipet.Office
{
    class SafeExcelMacroRunner : ISafeExcelMacroRunner
    {
        private readonly Application xApplication = null;
        private readonly Workbooks xBooks = null;
        private readonly Dictionary<string, Workbook> xWorkbooks = null;

        public SafeExcelMacroRunner()
        {
            this.xApplication = new Application()
            {
                DisplayAlerts = false,
                Visible = false
            };
            this.xBooks = this.xApplication.Workbooks;
            this.xWorkbooks = new Dictionary<string, Workbook>();
        }

        public void Dispose()
        {
            if (this.xWorkbooks != null)
            {
                var keys = this.xWorkbooks.Keys.ToArray();
                foreach (var key in keys)
                {
                    if (this.xWorkbooks.TryGetValue(key, out var value))
                    {
                        try
                        {
                            value.Close(false);
                        }
                        catch (COMException cex) when (cex.ErrorCode == -0x7FFE_FEF8)
                        {
                            Console.WriteLine($"{cex.Message}({cex.ErrorCode})");
                        }
                        finally
                        {
                            Marshal.FinalReleaseComObject(value);
                            this.xWorkbooks.Remove(key);
                        }
                    }
                }
            }

            if(this.xBooks != null)
            {
                Marshal.FinalReleaseComObject(this.xBooks);
            }

            if(this.xApplication != null)
            {
                try
                {
                    this.xApplication.Quit();
                }
                catch (COMException cex)
                {
                    Console.WriteLine($"{cex.Message}({cex.ErrorCode})");
                }
                finally
                {
                    Marshal.FinalReleaseComObject(this.xApplication);
                }
            }
        }

        public void OpenFile(string path)
        {
            var fileName = Path.GetFileName(path);
            if(this.xWorkbooks.ContainsKey(fileName))
            {
                throw new ArgumentException($"{fileName} is already opened.");
            }

            this.xWorkbooks.Add(fileName, this.xBooks.Open(path));
        }

        public void Run(string bookName, string macroName)
        {
            if (this.xWorkbooks.ContainsKey(bookName))
            {
                var name = $"{bookName}!{macroName}";
                this.xApplication.Run(name);
                return;
            }

            throw new ArgumentException($"{bookName} is not opened.");
        }

        public void Save(string bookName)
        {
            if(this.xWorkbooks.TryGetValue(bookName, out var book))
            {
                book?.Save();
                return;
            }

            throw new ArgumentException($"{bookName} is not opened.");
        }
    }
}
