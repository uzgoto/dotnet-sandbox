using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal class Interceptor
    {
        private static readonly Dictionary<string, string> EventKeys = new Dictionary<string, string>()
        {
            // Control
            {"Click", "EventClick" },
            {"TextChanged", "EventText" },
            // Form
            {"Load", "EVENT_LOAD" },
            // CheckBox
            {"CheckedChanged", "EVENT_CHECKEDCHANGED" },
            {"CheckStateChanged", "EVENT_CHECKSTATECHANGED" },
            // ListBox
            {"SelectedIndexChanged", "EVENT_SELECTEDINDEXCHANGED" },
            {"SelectedValueChanged", "EVENT_SELECTEDVALUECHANGED" },
            // ComboBox
            {"TextUpdate", "EVENT_TEXTUPDATE" },
        };

        private static FieldInfo EventClickField =>
            typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static);
        private static EventInfo ControlClickEvent =>
            typeof(Control).GetEvent("Click");
        private static PropertyInfo ControlEventsProperty =>
            typeof(Control).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);

        private EventHandler PreHandler { get; set; }
        private EventHandler PostHandler { get; set; }
        private Dictionary<Control, List<Delegate>> OriginalDelegates { get; set; }

        private Interceptor() { }

        public static Interceptor Create(IEnumerable<Control> targetControls, EventHandler preHandler, EventHandler postHandler)
        {
            return
                new Interceptor()
                {
                    OriginalDelegates = targetControls.ToDictionary(control => control, _ => new List<Delegate>()),
                    PreHandler = preHandler,
                    PostHandler = postHandler,
                };
        }

        public void InterceptClickEvent()
        {
            foreach (var control in this.OriginalDelegates.Keys)
            {
                // Get EventHandlerList from Control.Events property.
                var eventHandlers = ControlEventsProperty.GetValue(control, null) as EventHandlerList;
                // Get click eventHandler key object.
                var eventKey = EventClickField.GetValue(control);

                // Get eventHandler.
                var clickEventHandler = eventHandlers[eventKey];
                if (clickEventHandler != null)
                {
                    var delegates = clickEventHandler.GetInvocationList();
                    this.OriginalDelegates[control].AddRange(delegates);

                    Array.ForEach(delegates, dlgt => eventHandlers.RemoveHandler(eventKey, dlgt));

                    ControlClickEvent.AddEventHandler(control, this.PreHandler);
                    ControlClickEvent.AddEventHandler(control, clickEventHandler);
                    ControlClickEvent.AddEventHandler(control, this.PostHandler);
                }
            }
        }
    }
}
