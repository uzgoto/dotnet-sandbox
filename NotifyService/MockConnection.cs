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
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly Log Log;

        private bool _Connectable = true;
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

        public Task StartContinuousStatusSwitching(int delaySeconds, CancellationToken token)
        {
            return
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        this.Log.WriteLine($"begin delay {delaySeconds,2} seconds.");
                        Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                        this.Log.WriteLine($"end   delay {delaySeconds,2} seconds.");

                        this.Log.WriteLine($"begin switch from {(this.Connectable ? "connect" : "disconnect")}");
                        this.Connectable = !this.Connectable;
                        this.Log.WriteLine($"end   switch to   {(this.Connectable ? "connect" : "disconnect")}");
                    }
                }, token);
        }
    }
}
