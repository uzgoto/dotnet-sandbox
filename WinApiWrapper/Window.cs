using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public class Window
    {
        public Process Process { get; protected set; }
        public IntPtr Handle { get; protected set; }
        public string Title { get; protected set; }
        public string ClassName { get; protected set; }

        public static IEnumerable<Window> Enumerate(Desktop desktop)
        {
            foreach (var handle in ApiWrapper.EnumDesktopWindows(desktop.Id))
            {
                yield return new Window()
                {
                    Process = handle.GetProcess(),
                    Handle = handle,
                    Title = handle.GetWindowText(),
                    ClassName = handle.GetClassName(),
                };
            }
        }

        public static IEnumerable<string> EnumerateAll()
        {
            foreach (var winSta in WindowStation.Enumerate())
            {
                foreach (var desktop in Desktop.Enumerate(winSta))
                {
                    foreach (var window in Window.Enumerate(desktop))
                    {
                        yield return $"{winSta}, {desktop}, {window}";
                    }
                }
            }
        }

        public static IEnumerable<(int, string, int, string)> EnumerateWTSProcesses()
        {
            var sessions = ApiWrapper.WTSEnumerateSessions();
            var processes = ApiWrapper.WTSEnumerateProcesses();
            var zipped = Enumerable.Zip(sessions, processes, (s, p) => (s.Item1, s.Item2, p.Item2, p.Item3));
            foreach (var (sid, winsta, pid, procname) in zipped)
            {
                yield return (sid, winsta, pid, procname);
            }
        }

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
                    Process = parent,
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
                $"Process: {this.Process.ProcessName} ({this.Process.Id}), " +
                $"Window: {this.Title} ({this.Handle}) [{this.ClassName}]";
        }
    }
}
