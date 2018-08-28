using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessions = ApiWrapper.WTSEnumerateSessions();
            foreach (var (id, name) in sessions)
            {
                Console.WriteLine($"Popup to {name} ({id})");
                ApiWrapper.WTSSendMessage(id, "test", "testWindow");
            }

            var processes = ApiWrapper.WTSEnumerateProcesses();
            foreach (var (sid, pid, name) in processes)
            {
                Console.WriteLine($"{sid}: {name,-20} ({pid})");
            }

            foreach (var (hWinSta, winSta) in ApiWrapper.EnumWindowStations())
            {
                foreach(var (hDesktop, desktop) in ApiWrapper.EnumDesktops(hWinSta))
                {
                    foreach (var windowHandle in ApiWrapper.EnumDesktopWindows(hDesktop))
                    {
                        Console.Write($"{winSta,-7}({hWinSta,3})");
                        Console.Write($"/{desktop,-7}({hDesktop,3})");
                        Console.Write($", Process: {windowHandle.GetProcess().ProcessName,-20} ({windowHandle.GetProcess().Id,-5})");
                        Console.Write($", Window: {windowHandle.GetWindowText(),-20} [{windowHandle.GetClassName(),-25}] ({windowHandle,-8})");
                        Console.WriteLine();                        
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
