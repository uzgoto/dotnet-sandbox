using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class ConnectionWatcher
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly Log Log;
        private readonly Stopwatch StopWatch;
        private readonly MockConnection Connection;
        private readonly CancellationTokenSource CancellationTokenSource;

        private bool continuous = true;

        public ConnectionWatcher(Log log)
        {
            this.Log = log;
            this.StopWatch = new Stopwatch();
            this.Connection = new MockConnection(this.Log);
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        public void WatchContinuous()
        {
            this.StopWatch.Start();

            this.Connection.StartContinuousStatusSwitching(10, this.CancellationTokenSource.Token);

            var previousConnected = false;
            while (true)
            {
                Semaphore.Wait();
                if (!this.continuous)
                {
                    break;
                }

                var currentConnected = this.Connection.Connectable;

                // Close if any messageboxes showed, and show messagebox asynchronously, by service account.
                if (!previousConnected && currentConnected)
                {
                    this.CloseNotify();
                    this.Notify(Level.Information);
                }
                else if (previousConnected && !currentConnected)
                {
                    this.CloseNotify();
                    this.Notify(Level.Warning);
                }
                previousConnected = currentConnected;
                Semaphore.Release();

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
            this.Log.WriteLine($"End to watch.");
        }

        public void StopToWatch()
        {
            Semaphore.Wait();
            this.CancellationTokenSource.Cancel();
            this.continuous = false;
            Semaphore.Release();
        }

        private void CloseNotify()
        {
            this.Log.WriteLine($"begin close");
            foreach (var dialogInfo in ServiceNotifyDialog.EnumDialogs())
            {
                this.Log.WriteLine(dialogInfo);
            }
            try
            {
                ServiceNotifyDialog.Close();
            }
            catch (NullReferenceException nre)
            {
                this.Log.WriteLine($"{nre.Message} No dialog to close.");
            }
            catch (Exception ex)
            {
                Array.ForEach(
                    ex.ToString().Split('\n'),
                    line => this.Log.WriteLine(line));
            }
            this.Log.WriteLine($"end   close");
        }

        private void Notify(Level level)
        {
            this.Log.WriteLine($"begin {level}");
            switch (level)
            {
                case Level.Information:
                    //SystemNotifyDialog.ShowInformationAsync(
                    //    $"Connection reopened at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                    //    $"{Process.GetCurrentProcess().ProcessName}");
                    ServiceNotifyDialog.Show(
                        $"Connection reopened at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}");
                    break;
                case Level.Warning:
                    //SystemNotifyDialog.ShowWarningAsync(
                    //    $"Connection broken at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}",
                    //    $"{Process.GetCurrentProcess().ProcessName}");
                    ServiceNotifyDialog.Show(
                        $"Connection broken at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}");
                    break;
                default:
                    break;
            }
            this.Log.WriteLine($"end   {level}");
        }

        private enum Level
        {
            Information,
            Warning,
        }
    }
}
