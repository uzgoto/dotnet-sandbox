using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.WinForms
{
    public class ProgressSample
    {
        enum DialogType
        {
            Simple,
            Progress,
            //SimpleCancellable,
            //ProgressCancellable,
        }

        class SampleDialog<T> : Form
        {
            private IContainer components;
            private Label title;
            private ProgressBar bar;
            private Label progressText;
            private Label description;
            //private Button cancel;

            private int currentStep = 0;

            public SampleDialog(DialogType type, int maximun, Task<T> task)
            {
                InitializeComponent(type);
                this.Load += async (s, e) =>
                {
                    //var cts = new CancellationTokenSource();
                    var progress = new Progress<ReportMessage>(ReportProgress);

                    this.bar.Maximum = maximun;

                    await task;
                };
            }

            private void InitializeComponent(DialogType type)
            {
                this.components = new Container();
                this.AutoScaleMode = AutoScaleMode.Font;
                this.ClientSize = new Size(800, 450);
                this.Text = nameof(SampleDialog<T>);

                this.title = new Label() { Dock = DockStyle.Top , Text = "Title" };
                this.bar = new ProgressBar()
                {
                    Dock = DockStyle.Fill,
                    Minimum = 0,
                    Step = 1,
                    Style = ProgressBarStyle.Continuous,
                    Value = 0,
                };
                this.progressText = new Label()
                {
                    Dock = DockStyle.Fill,
                    Text = $"{this.currentStep} / {this.bar.Maximum}",
                };
                this.description = new Label() { Dock = DockStyle.Bottom , Text = "Description of Task" };
                //this.cancel = new Button() { Dock = DockStyle.Bottom, Text = "Cancel" };

                switch (type)
                {
                    case DialogType.Simple:
                        this.bar.Style = ProgressBarStyle.Marquee;
                        //this.cancel.Visible = false;
                        break;
                    case DialogType.Progress:
                        //this.cancel.Visible = false;
                        break;
                    //case DialogType.SimpleCancellable:
                    //    this.bar.Style = ProgressBarStyle.Marquee;
                    //    this.cancel.Visible = true;
                    //    break;
                    //case DialogType.ProgressCancellable:
                    //    this.cancel.Visible = true;
                    //    break;
                    default:
                        throw new ArgumentException($"{nameof(type)}:{type} is undefined in {nameof(DialogType)}.");
                }

                this.components.Add(this.title);
                this.components.Add(this.bar);
                this.components.Add(this.progressText);
                this.components.Add(this.description);
                //this.components.Add(this.cancel);
            }

            private void ReportProgress(ReportMessage message)
            { 
                this.bar.PerformStep();
                this.currentStep++;
            }

            class ReportMessage
            {

            }


            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    this.components.Dispose();
                }
                base.Dispose(disposing);
            }

        }
    }
}
