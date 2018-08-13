using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Services
{
    class SampleServicetmp : SafeShutdownableServiceBase
    {
        public SampleServicetmp() : base(nameof(SampleServicetmp)) { }

        protected override void OnStart(string[] args)
        {
            while(true)
            {
                lock (this.Lock)
                {
                    var lastLine = "0";
                    try
                    {
                        lastLine = File.ReadLines(@"C:/Users/Honey/Desktop/test.txt", Encoding.Default).Last();
                    }
                    catch { }
                    int.TryParse(lastLine, out var lastValue);
                    var values =
                        Enumerable.Range(0, 999)
                            .Select(idx => $"{decimal.Truncate((lastValue + 1) / 1000).ToString("000")}{idx.ToString("000")}");
                    File.AppendAllLines(@"C:/Users/Honey/Desktop/test.txt", values, Encoding.Default);
                }
            }
        }

        protected override void OnStop()
        {
            // TODO: サービスを停止するのに必要な終了処理を実行するコードをここに追加します。
        }
    }
}
