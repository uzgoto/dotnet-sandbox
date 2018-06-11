using System;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Interceptors;

namespace Uzgoto.DotNetSnipet.Sample
{
    public class DerivedForm : BaseForm
    {
        [Validator(InputType.Alpha)]
        private TextBox textbox1 = new TextBox();
        [Validator(InputType.Numeric)]
        private TextBox textbox2 = new TextBox();
        private TextBox textbox3 = new TextBox();
        private Label label1 = new Label();
        [Intercept("Click")]
        private Button button1 = new Button();

        public DerivedForm() : base()
        {
            this.textbox1.Text = "a";
            this.textbox2.Text = "1";
            this.textbox3.Text = "b2";
            this.textbox3.Dock = DockStyle.Top;
            this.textbox2.Dock = DockStyle.Top;
            this.textbox1.Dock = DockStyle.Top;
            this.label1.Text = "Hello Proxy.";
            this.label1.Dock = DockStyle.Bottom;
            this.button1.Dock = DockStyle.Bottom;
            this.button1.Text = "Click";
            this.Controls.Add(textbox1);
            this.Controls.Add(textbox2);
            this.Controls.Add(textbox3);
            this.Controls.Add(label1);
            this.Controls.Add(button1);

            this.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.label1.Text = string.Empty;
            this.button1.Enabled = false;
            this.label1.Text = this.textbox1.Text;
            this.button1.Enabled = true;
        }
    }
}
