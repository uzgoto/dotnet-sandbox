using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Uzgoto.DotNetSnipet.WinForms.Extensions;
using Uzgoto.DotNetSnipet.WinForms.Interceptors.Sanitizers;

namespace Uzgoto.DotNetSnipet.WinForms.Interceptors
{
    internal partial class BaseForm : Form
    {
        // 割り込み処理追加を取り扱う.
        private readonly Interceptor _interceptor;
        // 入力値無害化対象のコントロールと属性のペア.
        private readonly IEnumerable<KeyValuePair<Control, SanitizerTargetAttribute>> _sanitizationTargets;
        // 元の入力値退避用.
        private readonly Dictionary<Control, string> _tmpValues;

        public BaseForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            this._sanitizationTargets = this.EnumerateControlsWith<SanitizerTargetAttribute>();
            this._tmpValues = this._sanitizationTargets.ToDictionary(entry => entry.Key, _ => string.Empty);
            this._interceptor =
                Interceptor.Create(
                    this.EnumerateControlsWith<InterceptEventAttribute>().Select(entry => entry.Key),
                    this.PreInvoke,
                    this.PostInvoke);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // イベントハンドラーの前後に PreInvoke 、直後に PostInvoke 処理を割り込ませます.
            this._interceptor.InterceptEvent(
                this.EnumerateControlsWith<InterceptEventAttribute>().Select(entry => entry.Key),
                this.PreInvoke,
                this.PostInvoke);
        }

        /// <summary>
        /// イベント処理の直前に割り込ませる処理です.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreInvoke(object sender, EventArgs e)
        {
            foreach (var entry in this._sanitizationTargets)
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
            foreach (var entry in this._sanitizationTargets)
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
