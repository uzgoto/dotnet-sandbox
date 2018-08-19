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
            ShowDialog(text, "からのメッセージ", IconStyle.Information);
            //RunMsgExe(text);
        }

        public static IEnumerable<string> EnumDialogs()
        {
            //foreach (var window in Dialog.Enumerate()
                            //.OrderBy(w => w.Process.ProcessName)
                            //                                    )
            //{
            //foreach (var window in Process.GetProcessesByName("CSRSS").SelectMany(p => Dialog.EnumerateChildsOf(p)))
            //{
            foreach (var window in Dialog.EnumerateChildsOf(Process.GetCurrentProcess()))
            {
                yield return window.ToString();
            }
        }

        public static void Close()
        {
            var dialogs =
                Dialog.Enumerate();
                //Process.GetProcessesByName("CSRSS").SelectMany(p => Dialog.EnumerateChildsOf(p));
                //Dialog.EnumerateChildsOf(Process.GetCurrentProcess());
            foreach (var dialog in dialogs)
            {
                dialog.Close(isSilentlyContinue: true);
            }
            //dialogs?.FirstOrDefault()?.Close(isSilentlyContinue: true);
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

        private enum IconStyle
        {
            None,
            Information,
            Warining,
            Error,
            Question,
        }
        private static void ShowDialog(string text, string caption, IconStyle style)
        {
            Task.Factory.StartNew(() =>
            {
                Dialog.ShowInformation(text, caption);  
            });
        }
    }
}
