using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Uzgoto.DotNetSnipet.Services
{
    public class SampleService : ServiceBase
    {
        public Predicate<string[]> ValidateArguments { get; protected set; }
        public object Lock { get; private set; }

        public static void Main(string[] args)
        {
            Run(new SampleService());
        }

        public SampleService() : base()
        {
            this.AutoLog = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
            this.ServiceName = nameof(SampleService);
            this.Lock = new object();
            this.ValidateArguments = vs => vs.Length == 0;
        }

        protected override void OnStart(string[] args)
        {
            if (!this.ValidateArguments(args))
            {
                this.WriteLog($"{this.ServiceName}: 起動パラメータが異常です。", string.Join(" ", args));
                return;
            }
            base.OnStart(args);
            this.WriteLog($"{this.ServiceName}: 起動しました。", string.Join(" ", args));

            while (true)
            {
                lock (this.Lock)
                {
                    var lastLine = "0";
                    try
                    {
                        lastLine = File.ReadLines(@"%USERPROFILE%/Desktop/test.txt", Encoding.Default).Last();
                    }
                    catch { }
                    int.TryParse(lastLine, out var lastValue);
                    var values =
                        Enumerable.Range(0, 999)
                            .Select(idx => $"{decimal.Truncate((lastValue + 1) / 1000).ToString("000")}{idx.ToString("000")}");
                    File.AppendAllLines(@"%USERPROFILE%/Desktop/test.txt", values, Encoding.Default);
                }
            }
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
