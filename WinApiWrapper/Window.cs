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

        private static Window CreateFrom(Process process)
        {
            var handle = process.MainWindowHandle;
            return new Window()
            {
                Process = process,
                Handle = handle,
                Title = SafeUserApi.GetWindowText(handle),
                ClassName = SafeUserApi.GetClassName(handle),
            };
        }
        private static Window CreateFrom(IntPtr handle)
        {
            return new Window()
            {
                Process = SafeUserApi.GetProcess(handle),
                Handle = handle,
                Title = SafeUserApi.GetWindowText(handle),
                ClassName = SafeUserApi.GetClassName(handle),
            };
        }

        public static Window GetMainWindowOf(Process process)
        {
            return CreateFrom(process);
        }

        public static IEnumerable<Window> Enumerate(Desktop desktop)
        {
            foreach (var handle in ApiWrapper.EnumDesktopWindows(desktop.Id))
            {
                yield return CreateFrom(handle);
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
            foreach (var handle in SafeUserApi.EnumWindowHandles(parent.MainWindowHandle))
            {
                yield return CreateFrom(handle);
            }
        }

        public void Close(bool isSilentlyContinue = false)
        {
            if (isSilentlyContinue)
            {
                SafeUserApi.SilentlyClose(this.Handle);
            }
            else
            {
                SafeUserApi.Close(this.Handle);
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
