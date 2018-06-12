using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators;
using Uzgoto.DotNetSnipet.WinForms.Extensions;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public partial class BaseForm : Form
    {
        private Interceptor _interceptor;
        private IEnumerable<(Control control, SanitizerTargetAttribute attribute)> _sanitizationTargets;

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            this._sanitizationTargets = this.EnumerateControlsWith<SanitizerTargetAttribute>();
            var trrigerControls = this.EnumerateControlsWith<InterceptEventAttribute>().Select(elem => elem.control);
            this._interceptor = Interceptor.Create(trrigerControls, Intercept);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this._interceptor.InterceptClickEvent(this);
        }

        private void Intercept(object sender, EventArgs e)
        {
            var msg =
                this._sanitizationTargets
                    .Select(target =>
                        $"{target.control.GetType().Name}, " +
                        $"{target.control.Text}, " +
                        $"{target.attribute.InputType.ToString()}, " +
                        $"Sanitized:{target.attribute.Sanitize(target.control.Text)}");
            MessageBox.Show(string.Join(Environment.NewLine, msg));

            this._interceptor.Invoke(sender, e);

            MessageBox.Show(string.Join(Environment.NewLine, msg));
        }

    }
}
