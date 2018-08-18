using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class Log
    {
        private static readonly SemaphoreSlim LockLogFile = new SemaphoreSlim(0, 1);
        private static readonly string LogDirectory =
            Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\Logs");

        private readonly string LogPath;

        internal enum Name
        {
            Service,
            Console,
        }

        static Log()
        {
            Directory.CreateDirectory(LogDirectory);
        }

        public Log(Name name)
        {
            this.LogPath = Path.Combine(LogDirectory, name.ToString() + ".log");
            this.Rotate(this.LogPath);
        }

        private void Rotate(string path)
        {
            if (File.Exists(path))
            {
                File.Move(path,
                    path.Replace(".log", $"{DateTime.Now.ToString(".yyyyMMddHHmmssffffff.log")}"));
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            LockLogFile.Wait();
            File.AppendAllText(this.LogPath,
                $"[{DateTime.Now.ToLongDateString()}] {string.Format(format, args)}",
                Encoding.Default);
            LockLogFile.Release();
        }
    }
}
