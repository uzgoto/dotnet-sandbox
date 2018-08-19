using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uzgoto.Dotnet.Sandbox.Winapi;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    public static class ServiceNotifyDialog
    {
        public static void Show(string text)
        {
            RunMsgExe(text);
        }

        public static IEnumerable<string> EnumDialogs()
        {
            foreach (var msgDialog in Window.Enumerate())
            {
                yield return msgDialog.ToString();
            }
        }

        public static void Close()
        {
            var msgDialogs = Process.GetProcessesByName("CSRSS").SelectMany(p => Dialog.EnumerateChildsOf(p));
            msgDialogs.FirstOrDefault().Close();
        }

        private static void RunMsgExe(string text)
        {
            var sysRoot = Environment.GetEnvironmentVariable("SystemRoot");
            var msgPath = Path.Combine(sysRoot, @"Sysnative\msg.exe");
            var info = new ProcessStartInfo()
            {
                FileName = msgPath,
                Arguments = @"* /time:0 " + text,
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "RunAs",
            };
            Process.Start(info);
        }
    }
}
