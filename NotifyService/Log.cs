using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class Log
    {
        private static readonly SemaphoreSlim LockLogFile = new SemaphoreSlim(1, 1);
        private static readonly string LogDirectory =
            Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\Logs");

        private readonly string LogPath = Path.Combine(LogDirectory, Name.Service.ToString() + ".log");

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
                    path.Replace(".log", $"{DateTime.Now.ToString(".yyyyMMddHHmmssffffff")}.log"));
            }
        }

        public void WriteLine(string text, [CallerMemberName]string methodName = null)
        {
            LockLogFile.Wait();
            File.AppendAllLines(this.LogPath,
                new[] { $"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}] [{methodName}] {text}" },
                Encoding.Default);
            LockLogFile.Release();
        }
    }
}
