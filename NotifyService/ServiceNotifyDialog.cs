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
    public class ServiceNotifyDialog
    {
        private Process proc;

        public static int ShowWTSMessageBox(int sessionId, string text, string caption, IconStyle style)
        {
            switch (style)
            {
                case IconStyle.Information:
                    return SafeWTSApi.WTSSendMessageInformation(sessionId, text, caption);
                case IconStyle.Warining:
                    return SafeWTSApi.WTSSendMessageWarning(sessionId, text, caption);
                case IconStyle.Error:
                default:
                    return 0;
            }
        }
        public enum IconStyle
        {
            Information,
            Warining,
            Error,
        }

        public static IEnumerable<string> EnumDialogs()
        {
            foreach (var window in Window.EnumerateAll())
            {
                yield return window.ToString();
            }
        }

        public static void Close()
        {
            var dialogs = Dialog.EnumerateAll();
            foreach (var dialog in dialogs)
            {
                dialog.Close(isSilentlyContinue: true);
            }
            //dialogs?.FirstOrDefault()?.Close(isSilentlyContinue: true);
        }

        public void Kill()
        {
            this.proc?.Kill();
        }
    }
}
