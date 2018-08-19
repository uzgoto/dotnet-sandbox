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
                    this.Log.WriteLine($"Start to Switch.");
                    while (true)
                    {
                        if(token.IsCancellationRequested)
                        {
                            break;
                        }

                        this.Connectable = !this.Connectable;
                        Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
                    }
                    this.Log.WriteLine($"End to Switch.");
                }, token);
        }
    }
}
