using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uzgoto.Dotnet.Sandbox.Winapi;

namespace Uzgoto.Dotnet.Sandbox.ConsolePopup
{
    public static class SystemNotifyDialog
    {
        private static readonly MessageBoxButtons Buttons = MessageBoxButtons.OK;
        private static readonly MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;
        private static readonly MessageBoxOptions Options = MessageBoxOptions.ServiceNotification;

        public static Task<DialogResult> ShowInformationAsync(string text, string caption)
        {
            CloseAllDialogs();
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Information, DefaultButton, Options)
                );
        }
        
        public static Task ShowInformationAsync(string text)
        {
            CloseAllDialogs();
            return
                Task.Factory.StartNew(() =>
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
                    var proc = Process.Start(info);
                });
        }

        public static Task<DialogResult> ShowWarningAsync(string text, string caption)
        {
            CloseAllDialogs();
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Warning, DefaultButton, Options)
                );
        }

        public static Task ShowWarningAsync(string text)
        {
            CloseAllDialogs();
            Array.ForEach(Dialog.EnumerateChilds().ToArray(), d => d.Close());
            return
                Task.Factory.StartNew(() =>
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
                    var proc = Process.Start(info);
                });
        }

        private static void CloseAllDialogs()
        {
            Array.ForEach(Dialog.Enumerate().ToArray(), d => d.Close());
        }
    }
}
