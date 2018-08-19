using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public class Window
    {
        public string ProcessName { get; protected set; }
        public IntPtr Handle { get; protected set; }
        public string Title { get; protected set; }
        public string ClassName { get; protected set; }

        public static IEnumerable<Window> Enumerate()
        {
            foreach (var proc in Process.GetProcesses())
            {
                foreach (var window in EnumerateChildsOf(proc))
                {
                    yield return window;
                }
            }
        }

        public static IEnumerable<Window> EnumerateChilds()
        {
            foreach (var window in EnumerateChildsOf(Process.GetCurrentProcess()))
            {
                yield return window;
            }
        }

        public static IEnumerable<Window> EnumerateChildsOf(Process parent)
        {
            foreach (var handle in parent.MainWindowHandle.EnumWindowHandles())
            {
                yield return new Window()
                {
                    ProcessName = parent.ProcessName,
                    Handle = handle,
                    Title = handle.GetWindowText(),
                    ClassName = handle.GetClassName(),
                };
            }
        }

        public void Close(bool isSilentlyContinue = false)
        {
            if (isSilentlyContinue)
            {
                this.Handle.SilentlyClose();
            }
            else
            {
                this.Handle.Close();
            }
        }

        public override string ToString()
        {
            return
                string.Format("{{{0}:{1}, {2}:{3}, {4}:{5}, {6}:{7}}}",
                    nameof(this.ProcessName), this.ProcessName,
                    nameof(this.Handle), this.Handle,
                    nameof(this.Title), this.Title,
                    nameof(this.ClassName), this.ClassName);
        }
    }
}
