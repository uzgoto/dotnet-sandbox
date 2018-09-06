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
using Uzgoto.Dotnet.Sandbox.Winapi;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    partial class NotifyService : ServiceBase
    {
        private const int SERVICE_ACCEPT_PRESHUTDOWN = 0x0100;
        private const int SERVICE_CONTROL_PRESHUTDOWN = 0x000f;

        private readonly Log Log;
        private readonly ConnectionWatcher Watcher;

        private bool stopped = false;

        public NotifyService(Log log)
        {
            this.InitializeComponent();

            this.AcceptPreshutdown();

            this.Log = log;
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

        internal void OnStartByConsole(string[] args) => this.OnStart(args);

        internal void OnStopByConsole() => this.OnStop();

        protected override void OnStart(string[] args)
        {
            this.Log.WriteLine("OnStart.");
            Task.Factory.StartNew(() => this.Watcher.WatchContinuous());
        }

        protected override void OnStop()
        {
            this.Log.WriteLine("OnStop.");
            if (this.stopped)
            {
                this.Log.WriteLine($"OnStop is skipped.");
                return;
            }
            this.Watcher.StopToWatch();
            this.stopped = true;
            this.Log.WriteLine("Stopped.");
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

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            var sessionId = changeDescription.SessionId;
            var user = Session.GetUser(changeDescription.SessionId);
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.ConsoleConnect:
                    break;
                case SessionChangeReason.ConsoleDisconnect:
                    break;
                case SessionChangeReason.RemoteConnect:
                    break;
                case SessionChangeReason.RemoteDisconnect:
                    break;
                case SessionChangeReason.SessionLogon:
                    this.OnStop();
                    this.OnStart(null);
                    break;
                case SessionChangeReason.SessionLogoff:
                    this.OnStop();
                    this.OnStart(null);
                    break;
                case SessionChangeReason.SessionLock:
                    break;
                case SessionChangeReason.SessionUnlock:
                    break;
                case SessionChangeReason.SessionRemoteControl:
                    break;
                default:
                    break;
            }
        }
    }
}
