using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Uzgoto.DotNet.Sandbox.WinApiWrapper
{
    public class Window
    {
        public IntPtr Handle { get; private set; }
        public string Title { get; private set; }
        public string ClassName { get; private set; }

        public static IEnumerable<Window> EnumerateChildWindows(Process parent)
        {
            foreach (var handle in parent.MainWindowHandle.EnumWindowHandles())
            {
                yield return new Window()
                {
                    Handle = handle,
                    Title = handle.GetWindowText(),
                    ClassName = handle.GetClassName(),
                };
            }
        }

        public override string ToString()
        {
            return
                string.Format("{{{0}:{1}, {2}:{3}, {4}:{5}}}",
                    nameof(this.Handle), this.Handle,
                    nameof(this.Title), this.Title,
                    nameof(this.ClassName), this.ClassName);
        }

    }
}
