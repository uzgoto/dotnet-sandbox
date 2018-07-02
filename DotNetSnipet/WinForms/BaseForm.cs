using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Extensions;
using Uzgoto.DotNetSnipet.WinForms.Interceptors;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Sanitizers;

namespace Uzgoto.DotNetSnipet.WinForms
{
    public partial class BaseForm : Form
    {
        // For cached user inputted values.
        private Dictionary<Control, string> _tmpValues = new Dictionary<Control, string>();

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Interrupt pre/post eventhandler
            foreach (var (control, attribute) in this.EnumerateControlsWith<InterceptAttribute>())
            {
                if(control is Button button)
                {
                    Interceptor<Button>.Create(attribute.EventName).Intercept(button, this.PreInvoke, this.PostInvoke);
                }
                else if(control is ListBox listBox)
                {
                    Interceptor<ListBox>.Create(attribute.EventName).Intercept(listBox, this.PreInvoke, this.PostInvoke);
                }
                else if(control is ComboBox comboBox)
                {
                    Interceptor<ComboBox>.Create(attribute.EventName).Intercept(comboBox, this.PreInvoke, this.PostInvoke);
                }
            }
        }

        /// <summary>
        /// Invoke before event fire.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreInvoke(object sender, EventArgs e)
        {
            foreach (var (control, attribute) in this.EnumerateControlsWith<SanitizeAttribute>())
            {
                // Sanitize if user input is invalid.
                if(!attribute.IsValid(control.Text))
                {
                    this._tmpValues.Add(control, control.Text);
                    control.Text = attribute.Sanitize(control.Text);
                    MessageBox.Show($"{this._tmpValues[control]} is sanitized {control.Text}");
                }
            }
        }

        /// <summary>
        /// Invoke after event fired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostInvoke(object sender, EventArgs e)
        {
            foreach (var entry in this._tmpValues)
            {
                // Reset value user inputted if sanitized.
                var control = entry.Key;
                var value = entry.Value;

                control.Text = value;
            }
            this._tmpValues.Clear();
        }
    }
}
