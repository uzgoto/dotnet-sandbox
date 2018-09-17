using System;
using System.Collections.Generic;
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
        private Application xApplication = null;
        private Workbooks xBooks = null;
        private Dictionary<string, Workbook> xWorkbooks = null;

        public SafeExcelMacroRunner()
        {
            this.xApplication = new Application();
            this.xApplication.DisplayAlerts = false;
            this.xApplication.Visible = false;
            this.xBooks = this.xApplication.Workbooks;
        }

        public void Dispose()
        {
            if (this.xWorkbooks != null)
            {
                foreach (var key in this.xWorkbooks.Keys)
                {
                    if (this.xWorkbooks.TryGetValue(key, out var value))
                    {
                        try
                        {
                            value.Close(false);
                        }
                        finally
                        {
                            Marshal.FinalReleaseComObject(value);
                            this.xWorkbooks.Remove(key);
                        }
                    }
                }
                this.xWorkbooks = null;
            }

            if(this.xBooks != null)
            {
                Marshal.FinalReleaseComObject(this.xBooks);
                this.xBooks = null;
            }

            if(this.xApplication != null)
            {
                try
                {
                    this.xApplication.Quit();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(this.xApplication);
                    this.xApplication = null;
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
