using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Settings
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var messages = MessageProvider.Load();
            foreach (var msg in messages)
            {
                System.Windows.Forms.MessageBox.Show($"{msg.Code} : {msg.Text}", msg.Title, msg.Buttons, msg.Icon);
            }
        }
    }
}
