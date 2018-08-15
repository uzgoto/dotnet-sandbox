using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.ConsolePopup
{
    public class Program
    {
        private static volatile bool canConnected = true;
        private static readonly object Lock = new object();
        private static readonly string LogFormat = "[{0,-10}][{1,-5}][{2,-10}] Elapsed:{3:hh\\:mm\\:ss\\.ffffff} {4}";
        private const int MainProcessDelaySeconds = 5;

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            // Switch 'canConnected' random value asynchronously.
            SwitchConnectionStatusAsync(sw);

            var previousConnected = false;
            while (true)
            {
                bool currentConnected;
                lock (Lock)
                {
                    currentConnected = canConnected;
                }

                if (currentConnected != previousConnected)
                {
                    if (currentConnected)
                    {
                        // Close if any messageboxes showed, and show messagebox asynchronously, by service account.
                        Console.WriteLine(LogFormat, "MainProc", "begin", "open", sw.Elapsed, "Information");
                        SystemNotifyDialog.ShowInformationAsync(
                            $"Connection reopened at {DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                            $"{Process.GetCurrentProcess().ProcessName}");
                        Console.WriteLine(LogFormat, "MainProc", "end", "open", sw.Elapsed, "Information");
                    }
                    else
                    {
                        // Close if any messageboxes showed, and show messagebox asynchronously, by service account.
                        Console.WriteLine(LogFormat, "MainProc", "begin", "open", sw.Elapsed, "Warning");
                        SystemNotifyDialog.ShowWarningAsync(
                            $"Connection broken at {DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                            $"{Process.GetCurrentProcess().ProcessName}");
                        Console.WriteLine(LogFormat, "MainProc", "end", "open", sw.Elapsed, "Warning");
                    }
                }

                // Delay.
                //Console.WriteLine(LogFormat, "MainProc", "begin", "delay", sw.Elapsed, MainProcessDelaySeconds);
                Task.Delay(TimeSpan.FromSeconds(MainProcessDelaySeconds)).Wait();
                //Console.WriteLine(LogFormat, "MainProc", "end", "delay", sw.Elapsed, MainProcessDelaySeconds);

                previousConnected = currentConnected;
            }
        }

        private static void SwitchConnectionStatusAsync(Stopwatch sw)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var randomValue = new Random(sw.Elapsed.Milliseconds).Next();
                    var delaySeconds = randomValue % (30 - MainProcessDelaySeconds) + MainProcessDelaySeconds;

                    Console.WriteLine(LogFormat, "SwitchProc", "begin", "delay", sw.Elapsed, delaySeconds);
                    Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                    Console.WriteLine(LogFormat, "SwitchProc", "end", "delay", sw.Elapsed, delaySeconds);

                    lock (Lock)
                    {
                        Console.WriteLine(LogFormat, "SwitchProc", "begin", "switch", sw.Elapsed, canConnected);
                        canConnected = delaySeconds % 2 == 0;
                        Console.WriteLine(LogFormat, "SwitchProc", "end", "switch", sw.Elapsed, canConnected);
                    }
                }
            });
        }
    }
}
