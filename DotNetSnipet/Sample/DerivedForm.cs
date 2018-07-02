using System;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms;
using Uzgoto.DotNetSnipet.WinForms.Interceptors;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Sanitizers;

namespace Uzgoto.DotNetSnipet.Sample
{
    public class DerivedForm : BaseForm
    {
        [Sanitize(InputType.Alpha)]
        private TextBox textbox1 = new TextBox();
        [Sanitize(InputType.Numeric)]
        private TextBox textbox2 = new TextBox();
        private TextBox textbox3 = new TextBox();
        private GroupBox groupBox = new GroupBox();
        [Sanitize(InputType.Alpha)]
        private TextBox textbox4 = new TextBox();
        private Label label1 = new Label();
        [Intercept("Click")]
        private Button button1 = new Button();

        public DerivedForm() : base()
        {
            this.textbox1.Text = "a";
            this.textbox2.Text = "1";
            this.textbox3.Text = "b2";
            this.textbox4.Text = "32";
            this.groupBox.Dock = DockStyle.Top;
            this.textbox3.Dock = DockStyle.Top;
            this.textbox2.Dock = DockStyle.Top;
            this.textbox1.Dock = DockStyle.Top;
            this.label1.Text = "Hello Proxy.";
            this.label1.Dock = DockStyle.Bottom;
            this.button1.Dock = DockStyle.Bottom;
            this.button1.Text = "Click";

            this.SuspendLayout();
            this.groupBox.Controls.Add(textbox4);

            this.Controls.Add(textbox1);
            this.Controls.Add(textbox2);
            this.Controls.Add(textbox3);
            this.Controls.Add(groupBox);
            this.Controls.Add(label1);
            this.Controls.Add(button1);
            this.ResumeLayout();

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
