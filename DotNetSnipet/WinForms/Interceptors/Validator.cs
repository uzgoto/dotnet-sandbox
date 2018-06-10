using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public class BaseForm : Form
    {
        private static FieldInfo eventClickField = typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static);
        private static EventInfo clickEvent = typeof(Control).GetEvent(nameof(Click));
        private static PropertyInfo controlEvents = typeof(Control).GetProperty(nameof(Events), BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo interceptor = typeof(BaseForm).GetMethod(nameof(Validate), BindingFlags.Instance | BindingFlags.NonPublic);

        private Dictionary<Control, List<Delegate>> _handlers = new Dictionary<Control, List<Delegate>>();

        protected Dictionary<Control, InputType> ValidationTargets { get; set; }
        protected IEnumerable<Control> ValidationTrrigers { get; set; }

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var targetControls = this.EnumerateControlsWithAttribute<ValidatorAttribute>(this);
            this.ValidationTargets = targetControls.ToDictionary(ctrl => ctrl.Item1, ctrl => ctrl.Item2.InputType);

            var trrigerControls = this.EnumerateControlsWithAttribute<ValidationTrrigerAttribute>(this);
            this.ValidationTrrigers = trrigerControls.Select(ctrl => ctrl.Item1);

            if (this.ValidationTargets == null || this.ValidationTargets == null) return;

            foreach (var control in this.ValidationTrrigers)
            {
                var handlers = controlEvents.GetValue(control, null) as EventHandlerList;
                var eventKey = eventClickField.GetValue(control);
                var clickEventHandler = handlers[eventKey];
                if(clickEventHandler != null)
                {
                    var clickDelegates = clickEventHandler.GetInvocationList();
                    if(!this._handlers.ContainsKey(control))
                    {
                        this._handlers.Add(control, new List<Delegate>());
                    }
                    this._handlers[control].AddRange(clickDelegates);

                    Array.ForEach(clickDelegates, dlgt => handlers.RemoveHandler(eventKey, dlgt));

                    clickEvent.GetAddMethod().Invoke(control, new object[] { Delegate.CreateDelegate(clickEvent.EventHandlerType, this, interceptor) });
                }
            }
        }

        private void Validate(object sender, EventArgs e)
        {
            var msg = this.ValidationTargets.Select(target => $"{target.Key.GetType().Name}, {target.Key.Text}, {target.Value.ToString()}");
            MessageBox.Show(string.Join(Environment.NewLine, msg));

            foreach (var dlgt in this._handlers[sender as Control])
            {
                dlgt.DynamicInvoke(sender, e);
            }
        }

        private IEnumerable<Tuple<Control, T>> EnumerateControlsWithAttribute<T>(Form form) where T : Attribute
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (form.Controls == null) throw new ArgumentException($"{nameof(form)} has no controls.");

            var controls = form.Controls.OfType<Control>();

            var fields = form.GetType().GetRuntimeFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(form);
                if(value is Control)
                {
                    if(field.GetCustomAttribute<T>() != null)
                    {
                        yield return Tuple.Create(value as Control, field.GetCustomAttribute<T>());
                    }
                }
            }
        }
    }

    public class DerivedForm : BaseForm
    {
        [Validator(InputType.Alpha)]
        private TextBox textbox1 = new TextBox();
        [Validator(InputType.Numeric)]
        private TextBox textbox2 = new TextBox();
        private TextBox textbox3 = new TextBox();
        private Label label1 = new Label();
        [ValidationTrriger]
        private Button button1 = new Button();

        public DerivedForm() : base()
        {
            this.textbox1.Text = "a";
            this.textbox2.Text = "1";
            this.textbox3.Text = "b2";
            this.textbox3.Dock = DockStyle.Top;
            this.textbox2.Dock = DockStyle.Top;
            this.textbox1.Dock = DockStyle.Top;
            this.label1.Text = "Hello Proxy.";
            this.label1.Dock = DockStyle.Bottom;
            this.button1.Dock = DockStyle.Bottom;
            this.button1.Text = "Click";
            this.Controls.Add(textbox1);
            this.Controls.Add(textbox2);
            this.Controls.Add(textbox3);
            this.Controls.Add(label1);
            this.Controls.Add(button1);

            this.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.label1.Text = string.Empty;
            this.button1.Enabled = false;
            this.label1.Text = this.textbox1.Text;
            this.button1.Enabled = true;
        }
    }

}
