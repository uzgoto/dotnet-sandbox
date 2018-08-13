using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Text;
using Uzgoto.DotNetSnipet.Settings;
using Uzgoto.DotNetSnipet.Settings.Messages;

namespace Uzgoto.DotNetSnipet.Services
{
    partial class SafeShutdownableServiceBase : ServiceBase
    {
        public Predicate<string[]> ValidateArguments { get; protected set; }
        public object Lock { get; private set; }
        protected new bool CanPauseAndContinue { get; private set; }
        protected new bool CanShutdown { get; private set; }
        protected new bool CanStop { get; private set; }
        protected new int ExitCode { get; private set; }
        protected new string ServiceName { get; private set; }
        protected IEnumerable<Message> Messages { get; private set; }

        private SafeShutdownableServiceBase()
        {
            InitializeComponent();
        }

        protected SafeShutdownableServiceBase(string serviceName) : this()
        {
            this.AutoLog = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
            this.ExitCode = 0;
            this.ServiceName = serviceName;
            this.Lock = new object();
            this.Messages = MessageProvider.Load();
        }

        protected override void OnStart(string[] args)
        {
            if (this.ValidateArguments(args))
            {
                this.WriteLog($"{this.ServiceName}: 起動パラメータが異常です。", string.Join(" ", args));
                return;
            }
            base.OnStart(args);
            this.WriteLog($"{this.ServiceName}: 起動しました。", string.Join(" ", args));
        }

        protected override void OnStop()
        {
            base.OnStop();
            this.WriteLog($"{this.ServiceName}: 停止しました。");
        }

        protected override void OnShutdown()
        {
            this.WriteLog($"{this.ServiceName}: シャットダウンを待機します。");
            lock (this.Lock)
            {
                this.OnStop();

                this.WriteLog($"{this.ServiceName}: シャットダウンします。");
                base.OnShutdown();
            }
        }

        protected void WriteLog(params string[] messages)
        {
            var path = $"%USERPROFILE%/Desktop/{DateTime.UtcNow.ToString("yyyyMMdd")}_{this.ServiceName}.log";
            File.AppendAllLines(path, messages, Encoding.Default);
        }
    }
}
