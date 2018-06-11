using System;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Interceptor.InterceptClickEvent(this);
        }
    }
}
