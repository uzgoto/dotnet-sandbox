using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Extensions;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Sanitizers;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    public partial class BaseForm : Form
    {
        // 割り込み処理追加を取り扱う.
        private readonly IEnumerable<IEventInterceptor<Control>> _interceptors;
        // 入力値無害化対象のコントロールと属性のペア.
        private readonly IEnumerable<KeyValuePair<Control, SanitizerTargetAttribute>> _targets;
        // イベント割込み処理対象のコントロール
        private readonly IEnumerable<KeyValuePair<Control, InterceptEventAttribute>> _trrigers;
        // 元の入力値退避用.
        private readonly Dictionary<Control, string> _tmpValues;

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            this._targets = this.EnumerateControlsWith<SanitizerTargetAttribute>();
            this._trrigers = this.EnumerateControlsWith<InterceptEventAttribute>();
            this._interceptors =
                this._trrigers.Select(tr => ControlEventInterceptor.Create(tr.Key, tr.Value.EventName));
            this._tmpValues = this._targets.ToDictionary(entry => entry.Key, _ => string.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // イベントハンドラーの前後に PreInvoke 、直後に PostInvoke 処理を割り込ませます.
            foreach (var interceptor in this._interceptors)
            {
                interceptor.Intercept(this.PreInvoke, this.PostInvoke);
            }
        }

        /// <summary>
        /// イベント処理の直前に割り込ませる処理です.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreInvoke(object sender, EventArgs e)
        {
            foreach (var entry in this._targets)
            {
                var control = entry.Key;
                var attribute = entry.Value;

                this._tmpValues[control] = control.Text;
                control.Text = attribute.Sanitize(control.Text);
            }
        }

        /// <summary>
        /// イベント処理の直後に割り込ませる処理です.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostInvoke(object sender, EventArgs e)
        {
            foreach (var entry in this._targets)
            {
                var control = entry.Key;
                var attribute = entry.Value;

                if (this._tmpValues.ContainsKey(control))
                {
                    control.Text = this._tmpValues[control];
                }
            }
        }

    }
}
