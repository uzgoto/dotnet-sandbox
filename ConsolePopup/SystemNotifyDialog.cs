using System;
using System.Collections.Generic;
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
            Array.ForEach(Dialog.EnumerateChilds().ToArray(), d => d.Close());
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Information, DefaultButton, Options)
                );
        }

        public static Task<DialogResult> ShowWarningAsync(string text, string caption)
        {
            Array.ForEach(Dialog.EnumerateChilds().ToArray(), d => d.Close());
            return
                Task.Factory.StartNew(() =>
                    MessageBox.Show(text, caption, Buttons, MessageBoxIcon.Warning, DefaultButton, Options)
                );
        }
    }
}
