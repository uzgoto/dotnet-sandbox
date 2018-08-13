using System.IO;
using System.Linq;
using System.Text;

namespace Uzgoto.DotNetSnipet.Services
{
    class SampleService : SafeShutdownableServiceBase
    {
        public SampleService(string serviceName) : base(serviceName) { }

        protected override void OnStart(string[] args)
        {
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
    }
}
