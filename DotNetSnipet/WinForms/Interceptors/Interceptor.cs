using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal sealed class Interceptor<T> : IInterceptor<T> where T : Control
    {
        private static readonly Dictionary<string, string> EventKeys =
            new Dictionary<string, string>()
            {
                // Form
                { "Load", "EVENT_LOAD" },
                // CheckBox
                { "CheckStateChanged", "EVENT_CHECKSTATECHANGED" },
                // ListBox
                { "SelectedIndexChanged", "EVENT_SELECTEDINDEXCHANGED" },
                { "SelectedValueChanged", "EVENT_SELECTEDVALUECHANGED" },
                // ComboBox
                { "TextUpdate", "EVENT_TEXTUPDATE" },
                // RadioButton
                { "CheckedChanged", "EVENT_CHECKEDCHANGED" },
                // ScrollBar
                { "Scroll", "EVENT_SCROLL" },
                { "ValueChanged", "EVENT_VALUECHANGED" },
            };
        private static EventInfo ClickEvent = typeof(Control).GetEvent("Click");
        private static FieldInfo ClickEventKey =
            typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static);
        private static EventInfo TextChangedEvent = typeof(Control).GetEvent("TextChanged");
        private static FieldInfo TextChangedEventKey =
            typeof(Control).GetField("EventText", BindingFlags.NonPublic | BindingFlags.Static);
        private static PropertyInfo ControlEventProperty =>
            typeof(Control).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly EventInfo _controlEvent;
        private readonly FieldInfo _eventKeyField;

        private Interceptor() { }
        private Interceptor(string eventName)
        {
            switch (eventName)
            {
                case "Click":
                    this._controlEvent = ClickEvent;
                    this._eventKeyField = ClickEventKey;
                    break;
                case "TextChanged":
                    this._controlEvent = TextChangedEvent;
                    this._eventKeyField = TextChangedEventKey;
                    break;
                default:
                    this._controlEvent = typeof(T).GetEvent(eventName);
                    this._eventKeyField =
                        typeof(T).GetField(EventKeys[eventName], BindingFlags.NonPublic | BindingFlags.Static);
                    break;
            }
        }

        public static Interceptor<T> Create(string eventName)
        {
            return new Interceptor<T>(eventName);
        }

        public void Intercept(T instance, EventHandler preInvoke, EventHandler postInvoke)
        {
            if(this._eventKeyField == null || this._controlEvent == null)
            {
                throw new InvalidOperationException();
            }

            // Get EventHandlerList from Control.Events property.
            var eventHandlers = ControlEventProperty.GetValue(instance, null) as EventHandlerList;
            // Get click eventHandler key object.
            var eventKey = _eventKeyField.GetValue(instance);

            // Get eventHandler.
            var clickEventHandler = eventHandlers[eventKey];
            if (clickEventHandler != null)
            {
                this._controlEvent.RemoveEventHandler(instance, clickEventHandler);

                this._controlEvent.AddEventHandler(instance, preInvoke);
                this._controlEvent.AddEventHandler(instance, clickEventHandler);
                this._controlEvent.AddEventHandler(instance, postInvoke);
            }
        }
    }
}
