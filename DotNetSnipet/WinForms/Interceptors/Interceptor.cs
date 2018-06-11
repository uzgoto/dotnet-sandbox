using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Validators;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal class Interceptor
    {
        private static FieldInfo EventClickField =>
           typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static);
        private static EventInfo ControlClickEvent => typeof(Control).GetEvent("Click");
        private static PropertyInfo ControlEventsProperty =>
            typeof(Control).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo InterceptMethod =>
            typeof(Interceptor).GetMethod(nameof(Intercept), BindingFlags.Instance | BindingFlags.NonPublic);

        private static Dictionary<Control, SanitizerTargetAttribute> SanitizationTargets { get; set; }
        private static IEnumerable<Control> Interceptors { get; set; }
        private static Dictionary<Control, List<Delegate>> TmpHandlers { get; set; }

        private Interceptor() { }

        public static void InterceptClickEvent(Form form)
        {
            SanitizationTargets =
                EnumerateControls<SanitizerTargetAttribute>(form)
                    .ToDictionary(
                        target => target.control,
                        target => target.attribute);
            if (SanitizationTargets == null) return;

            Interceptors =
                EnumerateControls<InterceptAttribute>(form)
                    .Select(ctrl => ctrl.control);
            if (Interceptors == null) return;

            foreach (var control in Interceptors)
            {
                // Get EventHandlerList from Control.Events property.
                var eventHandlers = ControlEventsProperty.GetValue(control, null) as EventHandlerList;
                var eventKey = EventClickField.GetValue(control);
                // Get Click EventHandler.
                var clickEventHandler = eventHandlers[eventKey];
                if (clickEventHandler != null)
                {
                    var clickDelegates = clickEventHandler.GetInvocationList();
                    if (TmpHandlers == null) TmpHandlers = new Dictionary<Control, List<Delegate>>();
                    if (!TmpHandlers.ContainsKey(control))
                    {
                        TmpHandlers.Add(control, new List<Delegate>());
                    }
                    TmpHandlers[control].AddRange(clickDelegates);

                    Array.ForEach(clickDelegates, dlgt => eventHandlers.RemoveHandler(eventKey, dlgt));

                    ControlClickEvent.GetAddMethod().Invoke(
                        control,
                        new object[] { Delegate.CreateDelegate(ControlClickEvent.EventHandlerType, form, InterceptMethod) });
                }
            }
        }

        private static IEnumerable<(Control control, TAttribute attribute)> EnumerateControls<TAttribute>(Form form) where TAttribute : Attribute
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

        private void Intercept(object sender, EventArgs e)
        {
            var msg = SanitizationTargets
                .Select(target => 
                $"{target.Key.GetType().Name}, " +
                $"{target.Key.Text}, " +
                $"{target.Value.InputType.ToString()}, " +
                $"Sanitized:{target.Value.Sanitize(target.Key.Text)}");
            MessageBox.Show(string.Join(Environment.NewLine, msg));

            foreach (var dlgt in TmpHandlers[sender as Control])
            {
                dlgt.DynamicInvoke(sender, e);
            }
        }
    }
}
