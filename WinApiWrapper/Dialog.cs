using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public class Dialog : Window
    {
        private static readonly string DialogClassName = "#32770";

        public static new IEnumerable<Window> Enumerate()
        {
            foreach (var window in Window.Enumerate())
            {
                if (window.ClassName == DialogClassName)
                {
                    yield return window;
                }
            }
        }
        
        public static new IEnumerable<Window> EnumerateAll()
        {
            foreach (var winSta in WindowStation.Enumerate())
            {
                foreach (var desktop in Desktop.Enumerate(winSta))
                {
                    foreach (var window in Window.Enumerate(desktop))
                    {
                        if (window.ClassName == DialogClassName)
                        {
                            yield return window;
                        }
                    }
                }
            }
        }

        public static new IEnumerable<Window> EnumerateChilds()
        {
            foreach (var window in Window.EnumerateChilds())
            {
                if (window.ClassName == DialogClassName)
                {
                    yield return window;
                }
            }
        }

        public static new IEnumerable<Window> EnumerateChildsOf(Process parent)
        {
            foreach (var handle in parent.MainWindowHandle.EnumWindowHandles())
            {
                if (handle.GetClassName() == DialogClassName)
                {
                    yield return new Dialog()
                    {
                        Process = parent,
                        Handle = handle,
                        Title = handle.GetWindowText(),
                        ClassName = DialogClassName,
                    };
                }
            }
        }
    }
}
