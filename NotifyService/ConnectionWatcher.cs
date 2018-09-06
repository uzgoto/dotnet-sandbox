using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using Uzgoto.Dotnet.Sandbox.Winapi;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class ConnectionWatcher
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly Log Log;
        private readonly Stopwatch StopWatch;
        private readonly MockConnection Connection;
        private readonly CancellationTokenSource CancellationTokenSource;
        private readonly ServiceNotifyDialog Dialog;
        internal volatile int SessionId;

        private bool continuous = true;

        public ConnectionWatcher(Log log)
        {
            this.Log = log;
            this.StopWatch = new Stopwatch();
            this.Connection = new MockConnection(this.Log);
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Dialog = new ServiceNotifyDialog();
            this.SessionId = Session.GetCurrentSessionId();
        }

        public void WatchContinuous()
        {
            this.StopWatch.Start();

            this.Connection.StartContinuousStatusSwitching(10, this.CancellationTokenSource.Token);

            var previousConnected = false;
            while (this.continuous)
            {
                Semaphore.Wait();
                this.SessionId = Session.GetCurrentSessionId();

                var currentConnected = this.Connection.Connectable;

                // Close if any messageboxes showed, and show messagebox asynchronously, by service account.
                if (!previousConnected && currentConnected)
                {
                    this.Log.WriteLine($"Current session: {this.SessionId}");
                    this.CloseNotify(this.SessionId);
                    this.Notify(this.SessionId, Level.Information);
                }
                else if (previousConnected && !currentConnected)
                {
                    this.Log.WriteLine($"Current session: {this.SessionId}");
                    this.CloseNotify(this.SessionId);
                    this.Notify(this.SessionId, Level.Warning);
                }
                previousConnected = currentConnected;
                Semaphore.Release();

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
            //this.Log.WriteLine($"End to watch.");
        }

        public void StopToWatch()
        {
            Semaphore.Wait();
            this.CancellationTokenSource.Cancel();
            this.continuous = false;
            Semaphore.Release();
        }

        private void CloseNotify(int sessionId)
        {
           foreach(var window in Window.EnumerateAll())
            {
                this.Log.WriteLine(window.ToString());
            }
        }
        //private void CloseNotify(int sessionId)
        //{
        //    var process = Process.GetProcesses().FirstOrDefault(p => p.SessionId == sessionId && p.ProcessName == "csrss");
        //    if(process != null)
        //    {
        //        this.Log.WriteLine($"Close window of ({process.Id})");
        //        try
        //        {
        //            SafeUserApi.Close(process.MainWindowHandle);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Log.WriteLine($"Exception {ex.Message}");
        //        }
        //    }
        //}

        private void Notify(int sessionId, Level level)
        {
            try
            {
                var textInfo = $"Connection reopened at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}";
                var textWarn = $"Connection broken at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")}";
                var caption = $"{Process.GetCurrentProcess().ProcessName}";
                var code = 0;
                switch (level)
                {
                    case Level.Information:
                        this.Log.WriteLine($"Notify information to session ({sessionId})");
                        code = ServiceNotifyDialog.ShowWTSMessageBox(sessionId, textInfo, caption, ServiceNotifyDialog.IconStyle.Information);
                        break;
                    case Level.Warning:
                        this.Log.WriteLine($"Notify warning to session ({sessionId})");
                        code = ServiceNotifyDialog.ShowWTSMessageBox(sessionId, textWarn, caption, ServiceNotifyDialog.IconStyle.Warining);
                        break;
                    default:
                        break;
                }
                if(code != 0)
                {
                    this.Log.WriteLine($"Win32Error {code}");
                }
            }
            catch (Exception ex)
            {
                this.Log.WriteLine(ex.ToString());
            }
        }

        private enum Level
        {
            Information,
            Warning,
        }
    }
}
