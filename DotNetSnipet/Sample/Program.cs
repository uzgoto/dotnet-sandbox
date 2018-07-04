using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Messages;

namespace Uzgoto.DotNetSnipet.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var messages = MessageProvider.Load();
            foreach (var msg in messages)
            {
                System.Windows.Forms.MessageBox.Show($"{msg.Code} : {msg.Text}", msg.Title, msg.Buttons, msg.Icon);
            }
            //Application.Run(new DerivedForm());
        }
    }
}
