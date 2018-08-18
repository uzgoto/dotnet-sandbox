using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    partial class NotifyService : ServiceBase
    {
        private const int SERVICE_ACCEPT_PRESHUTDOWN = 0x0100;
        private const int SERVICE_CONTROL_PRESHUTDOWN = 0x000f;

        private Log Log;
        private ConnectionWatcher Watcher;
        private bool stopped = false;

        public NotifyService()
        {
            this.InitializeComponent();

            this.AcceptPreshutdown();

            this.Log = new Log(Log.Name.Service);
            this.Watcher = new ConnectionWatcher(this.Log);
        }

        private void AcceptPreshutdown()
        {
            var field =
                typeof(ServiceBase).GetField("acceptedCommands",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var value = (int)field.GetValue(this);
            field.SetValue(this, value | SERVICE_ACCEPT_PRESHUTDOWN);
        }

        internal void OnStartByConsole(string[] args)
        {
            this.Log = new Log(Log.Name.Console);
            this.OnStart(args);
        }

        internal void OnStopByConsole()
        {
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            this.Log.WriteLine("OnStart.");
            this.Watcher.WatchContinuous();
        }

        protected override void OnStop()
        {
            if(this.stopped)
            {
                return;
            }
            this.Log.WriteLine("OnStop.");

            this.Watcher.StopToWatch();            

            this.stopped = true;
        }

        protected override void OnShutdown()
        {
            this.Log.WriteLine("OnShutdown.");
            this.OnStop();
        }

        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                case SERVICE_CONTROL_PRESHUTDOWN:
                    this.Log.WriteLine("OnPreshutdown.");
                    this.OnStop();
                    break;
                default:
                    base.OnCustomCommand(command);
                    break;
            }
        }
    }
}
