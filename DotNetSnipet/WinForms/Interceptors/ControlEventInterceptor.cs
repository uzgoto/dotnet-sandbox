using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal class ControlEventInterceptor : IEventInterceptor<Control>
    {
        private static Dictionary<string, string> EventKeys =>
            new Dictionary<string, string>()
            {
                // Control, "Click", "EventClick"
                // Control, "TextChanged", "EventText"
                // ComboBox, "SelectedIndexChanged", "EVENT_SELECTEDINDEXCHANGED"
                // ComboBox, "SelectedItemChanged", "EVENT_SELECTEDITEMCHANGED"
                // ComboBox, "TextUpdate", "EVENT_TEXTUPDATE"
                // RadioButton, "CheckedChanged", "EVENT_CHECKEDCHANGED"
                // ScrollBar, "Scroll", "EVENT_SCROLL"
                // ScrollBar, "ValueChanged", "EVENT_VALUECHANGED"

                // Control
                { "Click", "EventClick"},
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

        private static PropertyInfo ControlEventsProperty =>
            typeof(Control).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);

        private Control TrrigerControl { get; set; }
        private FieldInfo EventKeyField { get; set; }
        private EventInfo ControlEvent { get; set; }

        private ControlEventInterceptor() { }

        public static IEventInterceptor<Control> Create(Control control, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) throw new ArgumentNullException(nameof(eventName));
            var eventFieldName = EventKeys[eventName] ?? throw new ArgumentException("It cannot use this context.", nameof(eventName));

            return
                new ControlEventInterceptor()
                {
                    TrrigerControl = control,
                    EventKeyField = control.GetType().GetField(EventKeys[eventName], BindingFlags.NonPublic | BindingFlags.Static),
                    ControlEvent = control.GetType().GetEvent(eventName),
                };
        }

        void IEventInterceptor<Control>.Intercept(EventHandler preHandler, EventHandler postHandler)
        {
            this.Intercept(preHandler, postHandler);
        }

        private void Intercept(EventHandler preHandler, EventHandler postHandler)
        {
            // Get EventHandlerList from Control.Events property.
            var eventHandlers = ControlEventsProperty.GetValue(this.TrrigerControl, null) as EventHandlerList;
            // Get click eventHandler key object.
            var eventKey = EventKeyField.GetValue(this.TrrigerControl);

            // Get eventHandler.
            var clickEventHandler = eventHandlers[eventKey];
            if (clickEventHandler != null)
            {
                this.ControlEvent.RemoveEventHandler(this.TrrigerControl, clickEventHandler);

                this.ControlEvent.AddEventHandler(this.TrrigerControl, preHandler);
                this.ControlEvent.AddEventHandler(this.TrrigerControl, clickEventHandler);
                this.ControlEvent.AddEventHandler(this.TrrigerControl, postHandler);
            }
        }
    }

    internal class ControlEventInfo
    {
        public Type ControlType { get; private set; }
        public string EventName { get; private set; }
        public string EventFieldName { get; private set; }

        public static readonly IEnumerable<ControlEventInfo> Values;
        static ControlEventInfo()
        {
            Values = new List<ControlEventInfo>()
            {
                new ControlEventInfo(){ControlType = typeof(Control), EventName = "Click", EventFieldName = "EventClick"},
                new ControlEventInfo(){ControlType = typeof(ListBox), EventName = "SelectedIndexChanged", EventFieldName = "EVENTSELECTEDINDEXCHANGED"},
            }.AsEnumerable();
        }
    }
}
