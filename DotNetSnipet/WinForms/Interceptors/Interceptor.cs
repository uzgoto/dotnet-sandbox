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

        private static Dictionary<Control, List<Delegate>> OriginalDelegates { get; set; }

        private MethodInfo InterceptMethod { get; set; }

        private Interceptor() { }

        public static Interceptor Create(EventHandler handler)
        {
            return
                new Interceptor()
                {
                    InterceptMethod = handler.GetMethodInfo(),
                };
        }

        public void InterceptClickEvent(Form form, IEnumerable<Control> trrigerControls)
        {
            foreach (var control in trrigerControls)
            {
                // Get EventHandlerList from Control.Events property.
                var eventHandlers = ControlEventsProperty.GetValue(control, null) as EventHandlerList;
                var eventKey = EventClickField.GetValue(control);
                // Get Click EventHandler.
                var clickEventHandler = eventHandlers[eventKey];
                if (clickEventHandler != null)
                {
                    var clickDelegates = clickEventHandler.GetInvocationList();
                    if (OriginalDelegates == null) OriginalDelegates = new Dictionary<Control, List<Delegate>>();
                    if (!OriginalDelegates.ContainsKey(control))
                    {
                        OriginalDelegates.Add(control, new List<Delegate>());
                    }
                    OriginalDelegates[control].AddRange(clickDelegates);

                    Array.ForEach(clickDelegates, dlgt => eventHandlers.RemoveHandler(eventKey, dlgt));

                    ControlClickEvent.GetAddMethod().Invoke(
                        control,
                        new object[] { Delegate.CreateDelegate(ControlClickEvent.EventHandlerType, form, InterceptMethod) });
                }
            }
        }

        public void Invoke(object sender, EventArgs e)
        {
            foreach (var dlgt in OriginalDelegates[sender as Control])
            {
                dlgt.DynamicInvoke(sender, e);
            }
        }
    }
}
