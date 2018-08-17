using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace Uzgoto.Dotnet.Sandbox.ConsolePopup
{
    public class Program
    {
        private const int MainProcessDelaySeconds = 5;

        private static readonly object LockConnectable = new object();
        private static readonly string LogFormat = "[{0,-10}][{1,-5}][{2,-5}] Elapsed:{3:hh\\:mm\\:ss\\.ffffff} {4}";
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
                    Console.WriteLine(LogFormat, "SwitchProc", "begin", "delay", StopWatch.Elapsed, delaySeconds);
                    Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                    Console.WriteLine(LogFormat, "SwitchProc", "end", "delay", StopWatch.Elapsed, delaySeconds);

                    lock (LockConnectable)
                    {
                        Console.WriteLine(LogFormat, "SwitchProc", "begin", "switch", StopWatch.Elapsed, Connectable);
                        Connectable = !Connectable;
                        Console.WriteLine(LogFormat, "SwitchProc", "end", "switch", StopWatch.Elapsed, Connectable);
                    }
                }
            });
        }

        private static void Close()
        {
            Console.WriteLine(LogFormat, "MainProc", "begin", "close", StopWatch.Elapsed, string.Empty);
            SystemNotifyDialog.Close();
            Console.WriteLine(LogFormat, "MainProc", "end", "close", StopWatch.Elapsed, string.Empty);
        }

        private static void Open(Level level)
        {
            Console.WriteLine(LogFormat, "MainProc", "begin", "open", StopWatch.Elapsed, level);
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
            Console.WriteLine(LogFormat, "MainProc", "end", "open", StopWatch.Elapsed, level);
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
