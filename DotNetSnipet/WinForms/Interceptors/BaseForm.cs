using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public partial class BaseForm : Form
    {
        private static FieldInfo EventClickField =>
            typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static);
        private static EventInfo ControlClickEvent => typeof(Control).GetEvent(nameof(Click));
        private static PropertyInfo ControlEventsProperty =>
            typeof(Control).GetProperty(nameof(Events), BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo InterceptMethod =>
            typeof(BaseForm).GetMethod(nameof(Sanitaize), BindingFlags.Instance | BindingFlags.NonPublic);

        private Dictionary<Control, List<Delegate>> _tmpHandlers = new Dictionary<Control, List<Delegate>>();

        protected Dictionary<Control, InputType> SanitizationTargets { get; set; }
        protected IEnumerable<Control> Interceptors { get; set; }

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SanitizationTargets =
                this.EnumerateControls<SanitizerTargetAttribute>(this)
                    .ToDictionary(
                        target => target.control,
                        target => target.attribute.InputType);
            if (this.SanitizationTargets == null) return;

            this.Interceptors =
                this.EnumerateControls<InterceptAttribute>(this)
                    .Select(ctrl => ctrl.control);
            if (this.Interceptors == null) return;

            foreach (var control in this.Interceptors)
            {
                // Get EventHandlerList from Control.Events property.
                var eventHandlers = ControlEventsProperty.GetValue(control, null) as EventHandlerList;
                var eventKey = EventClickField.GetValue(control);
                // Get Click EventHandler.
                var clickEventHandler = eventHandlers[eventKey];
                if (clickEventHandler != null)
                {
                    var clickDelegates = clickEventHandler.GetInvocationList();
                    if (!this._tmpHandlers.ContainsKey(control))
                    {
                        this._tmpHandlers.Add(control, new List<Delegate>());
                    }
                    this._tmpHandlers[control].AddRange(clickDelegates);

                    Array.ForEach(clickDelegates, dlgt => eventHandlers.RemoveHandler(eventKey, dlgt));

                    ControlClickEvent.GetAddMethod().Invoke(
                        control,
                        new object[] { Delegate.CreateDelegate(ControlClickEvent.EventHandlerType, this, InterceptMethod) });
                }
            }
        }

        private void Sanitaize(object sender, EventArgs e)
        {
            var msg = this.SanitizationTargets.Select(target => $"{target.Key.GetType().Name}, {target.Key.Text}, {target.Value.ToString()}");
            MessageBox.Show(string.Join(Environment.NewLine, msg));

            foreach (var dlgt in this._tmpHandlers[sender as Control])
            {
                dlgt.DynamicInvoke(sender, e);
            }
        }

        private IEnumerable<(Control control, TAttribute attribute)> EnumerateControls<TAttribute>(Form form) where TAttribute : Attribute
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (form.Controls == null) throw new ArgumentException($"{nameof(form)} has no controls.");

            var controls = form.Controls.OfType<Control>();

            var fields = form.GetType().GetRuntimeFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(form);
                if (value is Control control)
                {
                    var attribute = field.GetCustomAttribute<TAttribute>();
                    if (attribute != null)
                    {
                        yield return (control, attribute);
                    }
                }
            }
        }
    }
}
