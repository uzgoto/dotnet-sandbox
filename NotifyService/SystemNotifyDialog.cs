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
    public static class SystemNotifyDialog
    {
        private static readonly MessageBoxButtons Buttons = MessageBoxButtons.OK;
        private static readonly MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;
        private static readonly MessageBoxOptions Options =
            MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly;

        public static Task<DialogResult> ShowInformationAsync(string text, string caption)
        {
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Information, DefaultButton, Options)
                );
        }
        
        public static Task<DialogResult> ShowWarningAsync(string text, string caption)
        {
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Warning, DefaultButton, Options)
                );
        }

        public static async void Show(string text)
        {
            await RunMsgExe(text);
        }

        public static void Close()
        {
            Array.ForEach(Dialog.Enumerate().ToArray(), d => d.Close());
        }

        private static Task<Process> RunMsgExe(string text)
        {
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
                    return Process.Start(info);
                });
        }
    }
}
