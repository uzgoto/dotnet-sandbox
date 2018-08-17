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

        private static readonly object LockLogFile = new object();
        private static readonly string LogDirectory =
            Path.Combine(
                Assembly.GetExecutingAssembly().Location,
                @"..\..\Logs");
        private static readonly string ServiceLogFile = "Service.log";
        private static readonly string ConsoleLogFile = "Console.log";

        private string LogPath = Path.Combine(LogDirectory, ServiceLogFile);

        public NotifyService()
        {
            this.InitializeComponent();

            var field =
                typeof(ServiceBase).GetField("acceptedCommands",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var value = (int)field.GetValue(this);
            field.SetValue(this, value | SERVICE_ACCEPT_PRESHUTDOWN);

            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        internal void OnStartByConsole(string[] args)
        {
            this.LogPath = this.LogPath.Replace(ServiceLogFile, ConsoleLogFile);
            this.OnStart(args);
        }

        internal void OnStopByConsole()
        {
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            this.WriteLogFile("Start service.");
        }

        protected override void OnStop()
        {
            this.WriteLogFile("Stop service.");
        }

        protected override void OnShutdown()
        {
            this.WriteLogFile("Shutdown.");
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == SERVICE_CONTROL_PRESHUTDOWN)
            {
                this.WriteLogFile("Preshutdown.");
                this.OnStop();
                return;
            }

            base.OnCustomCommand(command);
        }

        private void WriteLogFile(string contents)
        {
            lock (LockLogFile)
            {
                File.AppendAllText(this.LogPath, contents, Encoding.Default);
            }
        }
    }
}
