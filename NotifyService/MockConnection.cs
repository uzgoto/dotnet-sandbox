using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class MockConnection
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(0, 1);
        private static readonly string LogFormat = "[{0,-10}][{1,-5}][{2,-5}] {3}";

        private readonly Log Log;

        private bool _Connectable;
        public bool Connectable
        {
            get
            {
                Semaphore.Wait();
                var tmp = this._Connectable;
                Semaphore.Release();
                return tmp;
            }
            private set
            {
                Semaphore.Wait();
                this._Connectable = value;
                Semaphore.Release();
            }
        }

        public MockConnection(Log log)
        {
            this.Log = log;
        }

        public Task StartContinuousStatusSwitching(int delaySeconds)
        {
            return
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        this.Log.WriteLine(LogFormat, "SwitchProc", "begin", "delay", delaySeconds);
                        Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                        this.Log.WriteLine(LogFormat, "SwitchProc", "end", "delay", delaySeconds);

                        this.Log.WriteLine(LogFormat, "SwitchProc", "begin", "switch", this.Connectable);
                        this.Connectable = !this.Connectable;
                        this.Log.WriteLine(LogFormat, "SwitchProc", "end", "switch", this.Connectable);
                    }
                });
        }
    }
}
