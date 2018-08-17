using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    public class Program
    {
        private const int MainProcessDelaySeconds = 5;

        private static readonly object LockConnectable = new object();
        private static readonly string LogFormat = "{0:yyyy/MM/dd hh\\:mm\\:ss\\.ffffff} [{1,-10}][{2,-5}][{3,-5}] {4}";
        private static readonly Stopwatch StopWatch = new Stopwatch();
        private static readonly object LockLogFile = new object();
        private static readonly string LogPath =
            Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\Logs",
                Assembly.GetExecutingAssembly().GetName().Name + ".log");
        private static volatile bool Connectable = true;

        static void Main(string[] args)
        {
            StopWatch.Start();

            // Switch 'Connectable' value asynchronously.
            SwitchConnectableAsync(delaySeconds: 10);

            var previousConnected = false;
            while (true)
            {
                bool currentConnected;
                lock (LockConnectable)
                {
                    currentConnected = Connectable;
                }

                // Close if any messageboxes showed, and show messagebox asynchronously, by service account.
                if (!previousConnected && currentConnected)
                {
                    Close();
                    Open(Level.Information);
                }
                else if (previousConnected && !currentConnected)
                {
                    Close();
                    Open(Level.Warning);
                }

                Task.Delay(TimeSpan.FromSeconds(MainProcessDelaySeconds)).Wait();
                previousConnected = currentConnected;
            }
        }

        private static void SwitchConnectableAsync(int delaySeconds)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    WriteLogFile(LogFormat, DateTime.Now, "SwitchProc", "begin", "delay", delaySeconds);
                    Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                    WriteLogFile(LogFormat, DateTime.Now, "SwitchProc", "end", "delay", delaySeconds);

                    lock (LockConnectable)
                    {
                        WriteLogFile(LogFormat, DateTime.Now, "SwitchProc", "begin", "switch", Connectable);
                        Connectable = !Connectable;
                        WriteLogFile(LogFormat, DateTime.Now, "SwitchProc", "end", "switch", Connectable);
                    }
                }
            });
        }

        private static void Close()
        {
            WriteLogFile(LogFormat, DateTime.Now, "MainProc", "begin", "close", string.Empty);
            SystemNotifyDialog.Close();
            WriteLogFile(LogFormat, DateTime.Now, "MainProc", "end", "close", string.Empty);
        }

        private static void Open(Level level)
        {
            WriteLogFile(LogFormat, DateTime.Now, "MainProc", "begin", "open", level);
            switch (level)
            {
                case Level.Information:
                    //SystemNotifyDialog.ShowInformationAsync(
                    //    $"Connection reopened at {DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                    //    $"{Process.GetCurrentProcess().ProcessName}");
                    SystemNotifyDialog.Show(
                        $"Connection reopened at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}");
                    break;
                case Level.Warning:
                    //SystemNotifyDialog.ShowWarningAsync(
                    //    $"Connection broken at {DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                    //    $"{Process.GetCurrentProcess().ProcessName}");
                    SystemNotifyDialog.Show(
                        $"Connection broken at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}");
                    break;
                default:
                    break;
            }
            WriteLogFile(LogFormat, DateTime.Now, "MainProc", "end", "open", level);
        }

        private enum Level
        {
            Information,
            Warning,
        }

        private static void WriteLogFile(string format, params object[] parameters)
        {
            lock(LockLogFile)
            {
                File.AppendAllText(LogPath, string.Format(format, parameters), Encoding.Default);
            }
        }
    }
}
