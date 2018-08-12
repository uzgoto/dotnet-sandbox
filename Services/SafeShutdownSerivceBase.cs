using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Uzgoto.DotNetSnipet.Services
{
    abstract class SafeShutdownSerivceBase : ServiceBase
    {
        protected bool AllowShutdown { get; set; }
        protected new EventLog EventLog { get; set; }

        protected SafeShutdownSerivceBase(string serviceName) : base()
        {
            this.AutoLog = false;
            this.CanShutdown = true;
            this.CanStop = false;
            this.EventLog = new EventLog(logName: "CustomApp", machineName: "localhost", source: serviceName);
            this.ServiceName = serviceName;
        }

        protected abstract bool IsArgumentsValid(string[] args);

        protected override void OnShutdown()
        {
            while (!this.AllowShutdown)
            {
                this.EventLog.WriteEntry($"{this.ServiceName}: シャットダウンを待機します。", EventLogEntryType.Information, 0);
            }
            this.EventLog.WriteEntry($"{this.ServiceName}: シャットダウンします。", EventLogEntryType.Information, 0);
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            if(this.IsArgumentsValid(args))
            {
                this.EventLog.WriteEntry($"{this.ServiceName}: 起動パラメータが不正です。\n{string.Join("\n", args)}", EventLogEntryType.Error, 100);
                return;
            }
            base.OnStart(args);
            this.EventLog.WriteEntry($"{this.ServiceName}: 起動しました。\n{string.Join("\n", args)}", EventLogEntryType.Information, 0);
        }
    }
}
