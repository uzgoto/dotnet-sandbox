using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public partial class BaseForm : Form
    {
        Interceptor interceptor;
        IEnumerable<(Control control, SanitizerTargetAttribute attribute)> SanitizationTargets;

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            this.SanitizationTargets = EnumerateControls<SanitizerTargetAttribute>();
            this.interceptor = Interceptor.Create(Intercept);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.interceptor.InterceptClickEvent(this, EnumerateControls<InterceptEventAttribute>().Select(elem => elem.control));
        }

        private void Intercept(object sender, EventArgs e)
        {
            var msg =
                this.SanitizationTargets
                    .Select(target =>
                        $"{target.control.GetType().Name}, " +
                        $"{target.control.Text}, " +
                        $"{target.attribute.InputType.ToString()}, " +
                        $"Sanitized:{target.attribute.Sanitize(target.control.Text)}");
            MessageBox.Show(string.Join(Environment.NewLine, msg));

            this.interceptor.Invoke(sender, e);
        }

        private IEnumerable<(Control control, T attribute)> EnumerateControls<T>() where T : Attribute
        {
            if (this.Controls == null) throw new ArgumentException("This form has no controls.");

            var controls = this.Controls.OfType<Control>();

            var fields = this.GetType().GetRuntimeFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if (value is Control control)
                {
                    var attribute = field.GetCustomAttribute<T>();
                    if (attribute != null)
                    {
                        yield return (control, attribute);
                    }
                }
            }
        }

    }
}
