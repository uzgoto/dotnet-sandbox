using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Extensions;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal partial class BaseForm : Form
    {
        private Interceptor _interceptor;
        private IEnumerable<(Control control, SanitizerTargetAttribute attribute)> _sanitizationTargets;
        private Dictionary<Control, string> _tmpValues = new Dictionary<Control, string>();

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            this._sanitizationTargets = this.EnumerateControlsWith<SanitizerTargetAttribute>();
            var trrigerControls = this.EnumerateControlsWith<InterceptEventAttribute>().Select(elem => elem.control);
            this._interceptor = Interceptor.Create(trrigerControls, this.PreInvoke, this.PostInvoke);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this._interceptor.InterceptClickEvent();
        }

        private void PreInvoke(object sender, EventArgs e)
        {
            foreach (var (control, attribute) in this._sanitizationTargets)
            {
                this._tmpValues[control] = control.Text;
                control.Text = attribute.Sanitize(control.Text);
            }
        }

        private void PostInvoke(object sender, EventArgs e)
        {
            foreach (var (control, attribute) in this._sanitizationTargets)
            {
                if (this._tmpValues.ContainsKey(control))
                {
                    control.Text = this._tmpValues[control];
                }
            }
        }

    }
}
