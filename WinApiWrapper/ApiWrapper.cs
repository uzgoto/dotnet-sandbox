using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public static class ApiWrapper
    {
        private static class WinApi
        {
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nMaxCount);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern IntPtr OpenWindowStation(string lpszWinSta, bool fInherit, uint dwDesiredAccess);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern bool SetProcessWindowStation(IntPtr hWinSta);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern IntPtr OpenDesktop(string lpszDesktop, int dwFlags, bool fInherit, uint dwDesiredAccess);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern bool SetThreadDesktop(IntPtr hDesktop);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern IntPtr GetProcessWindowStation();
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, out int lpnLengthNeeded);
            internal enum UOI
            {
                FLAGS = 1,
                NAME = 2,
                TYPE = 3,
                USER_SID = 4,
                HEAPSIZE = 5,
                IO = 6,
            }

            #region WTS
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern bool WTSSendMessage(
                IntPtr hServer,
                [MarshalAs(UnmanagedType.I4)]int sessionId,
                string pTitle,
                [MarshalAs(UnmanagedType.U4)]int titleLength,
                string pMessage,
                [MarshalAs(UnmanagedType.U4)]int messageLength,
                [MarshalAs(UnmanagedType.U4)]uint style,
                [MarshalAs(UnmanagedType.U4)]int timeout,
                [MarshalAs(UnmanagedType.U4)]out int pResponse,
                bool bWait);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSEnumerateSessions(IntPtr hServer, int Reserved, int Version, out IntPtr ppSessionInfo, out int pCount);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSEnumerateProcesses(IntPtr hServer, int Reserved, int Version, out IntPtr ppProcessInfo, out int pCount);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSFreeMemory(IntPtr pMemory);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern bool WTSTerminateProcess(IntPtr hServer, int ProcessId, int ExitCode);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WTS_SESSION_INFO
            {
                public int SessionId { get; set; }
                public string WinStationName { get; set; }
                public WTS_CONNECTSTATE_CLASS State { get; set; }
            }
            [Flags]
            internal enum WTS_CONNECTSTATE_CLASS : uint
            {
                WTSActive,
                WTSConnected,
                WTSConnectQuery,
                WTSShadow,
                WTSDisconnected,
                WTSIdle,
                WTSListen,
                WTSReset,
                WTSDown,
                WTSInit
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct WTS_PROCESS_INFO
            {
                public int SessionId { get; set; }
                public int ProcessId { get; set; }
                public string ProcessName { get; set; }
                public string UserId { get; set; }
            }
            #endregion

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool EnumWindowStations(EnumWindowStationsDelegate lpEnumFunc, IntPtr lParam);
            internal delegate bool EnumWindowStationsDelegate(string windowStation, IntPtr lParam);
            internal static bool EnumWindowStationsCallback(string windowStation, IntPtr lParam)
            {
                var gch = GCHandle.FromIntPtr(lParam);
                if (gch.Target is List<string> list)
                {
                    list.Add(windowStation);
                    return true;
                }
                return false;
            }
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool EnumDesktops(IntPtr hWinSta, EnumDesktopsDelegate lpEnumFunc, IntPtr lParam);
            internal delegate bool EnumDesktopsDelegate(string lpszDesktop, IntPtr lParam);
            internal static bool EnumDesktopsCallback(string lpszDesktop, IntPtr lParam)
            {
                var gch = GCHandle.FromIntPtr(lParam);
                if (gch.Target is List<string> list)
                {
                    list.Add(lpszDesktop);
                    return true;
                }
                return false;
            }
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsDelegate lpfn, IntPtr lParam);
            internal delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);
            internal static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
            {
                var gch = GCHandle.FromIntPtr(lParam);
                if (gch.Target is List<IntPtr> list)
                {
                    list.Add(hWnd);
                    return true;
                }
                return false;
            }
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

            internal enum WM
            {
                CLOSE = 0x0002,
                SYSCOMMAND = 0x0112,
            }
            internal enum SC
            {
                CLOSE = 0xF060
            }
            [Flags]
            internal enum MB : uint
            {
                OK = 0x0000_0000,
                OKCANCEL = 0x0000_0001,
                ABORTRETRYIGNORE = 0x0000_00002,
                YESNOCANCEL = 0x0000_0003,
                YESNO = 0x0000_0004,
                RETRYCANCEL = 0x0000_0005,
                CANCELTRYCONTINUE = 0x0000_0006,
                MB_HELP = 0x0000_4000,

                ICONERROR = 0x0000_0010,
                ICONQUESTION = 0x0000_0020,
                ICONWARNING = 0x0000_0030,
                ICONINFORMATION = 0x0000_0040,

                APPLMODAL = 0x0000_0000,
                SYSTEMMODAL = 0x0000_1000,
                TASKMODAL = 0x0000_2000,

                SETFOREGROUND = 0x0001_0000,
                DEFAULT_DESKTOP_ONLY = 0x0002_0000,
                TOPMOST = 0x0004_0000,
                SERVICE_NOTIFICATION = 0x0020_0000,
            }

            [Flags]
            internal enum WINSTA : uint
            {
                ALL_ACCESS = 0x37F,
                ENUMDESKTOPS = 0x0001,
                READATTRIBUTES = 0x0002,
                ACCESSCLIPBOARD = 0x0004,
                CREATEDESKTOP = 0x0008,
                WRITEATTRIBUTES = 0x0010,
                ACCESSGLOBALATOMS = 0x0020,
                EXITWINDOWS = 0x0040,
                ENUMERATE = 0x0100,
                READSCREEN = 0x0200,
            }

            [Flags]
            internal enum DESKTOP : uint
            {
                READOBJECTS = 0x0001,
                CREATEWINDOW = 0x0002,
                CREATEMENU = 0x0004,
                HOOKCONTROL = 0x0008,
                JOURNALRECORD = 0x0010,
                JOURNALPLAYBACK = 0x0020,
                ENUMERATE = 0x0040,
                WRITEOBJECTS = 0x0080,
                SWITCHDESKTOP = 0x0100,
                ALL = READOBJECTS | CREATEWINDOW | CREATEMENU | HOOKCONTROL | JOURNALRECORD | JOURNALPLAYBACK | ENUMERATE | WRITEOBJECTS | SWITCHDESKTOP,
            }
        }

        #region HandleExtensions
        internal static IEnumerable<IntPtr> EnumWindowHandles(this IntPtr hWndRoot)
        {
            yield return hWndRoot;

            var child = IntPtr.Zero;
            while ((child = WinApi.FindWindowEx(hWndRoot, child, null, null)) != IntPtr.Zero)
            {
                foreach (var childchild in EnumWindowHandles(child))
                {
                    yield return childchild;
                }
            }
        }

        internal static string GetClassName(this IntPtr hWnd)
        {
            var builder = new StringBuilder(256);
            WinApi.GetClassName(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        internal static string GetWindowText(this IntPtr hWnd)
        {
            var builder = new StringBuilder(256);
            WinApi.GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        public static Process GetProcess(this IntPtr hWnd)
        {
            WinApi.GetWindowThreadProcessId(hWnd, out var processId);
            return Process.GetProcessById(processId);
        }

        internal static void Close(this IntPtr hWnd)
        {
            WinApi.SendMessage(hWnd, (int)WinApi.WM.SYSCOMMAND, (int)WinApi.SC.CLOSE, IntPtr.Zero);
            var code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new Win32Exception(code);
            }
        }

        internal static void SilentlyClose(this IntPtr hWnd)
        {
            WinApi.SendMessage(hWnd, (int)WinApi.WM.SYSCOMMAND, (int)WinApi.SC.CLOSE, IntPtr.Zero);
        }
        #endregion

        public static void ShowInformationMessageBox(string text, string caption) =>
            ShowMessageBox(text, caption, WinApi.MB.ICONINFORMATION);
        public static void ShowWarningMessageBox(string text, string caption) =>
            ShowMessageBox(text, caption, WinApi.MB.ICONWARNING);
        public static void ShowErrorMessageBox(string text, string caption) =>
            ShowMessageBox(text, caption, WinApi.MB.ICONERROR);

        private static void ShowMessageBox(string text, string caption, WinApi.MB iconStyle)
        {
            //SwitchWinStaAndDesktop();
            var type0001 = WinApi.MB.DEFAULT_DESKTOP_ONLY;
            var type0010 = WinApi.MB.SETFOREGROUND;
            var type0100 = WinApi.MB.TOPMOST;
            var type1000 = WinApi.MB.SERVICE_NOTIFICATION;
            var type0011 = type0001 | type0010;
            var type0101 = type0001 | type0100;
            var type1001 = type0001 | type1000;
            var type0110 = type0010 | type0100;
            var type1010 = type0010 | type1000;
            var type1100 = type0100 | type1000;
            var type0111 = type0100 | type0010 | type0001;
            var type1011 = type1000 | type0010 | type0001;
            var type1101 = type1000 | type0100 | type0001;
            var type1110 = type1000 | type0100 | type0010;
            var type1111 = type1000 | type0100 | type0010 | type0001;

            var type =
                WinApi.MB.OK |
                iconStyle |
                WinApi.MB.APPLMODAL |
                type0111;
            WinApi.MessageBox(IntPtr.Zero, text, caption, (uint)type);
            var code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new Win32Exception(code);
            }
        }

        private static void SwitchWinStaAndDesktop()
        {
            var hWinSta = WinApi.OpenWindowStation("WinSta0", false, (uint)WinApi.WINSTA.ALL_ACCESS);
            WinApi.SetProcessWindowStation(hWinSta);
            var hDesktop = WinApi.OpenDesktop("Default", 0, false, (uint)WinApi.DESKTOP.ALL);
            WinApi.SetThreadDesktop(hDesktop);
        }

        public static (string, int) Switch()
        {
            var ptr = default(IntPtr);
            try
            {
                var winsta = WinApi.GetProcessWindowStation();
                var uoi = WinApi.UOI.USER_SID;
                var needed = 0;
                switch (uoi)
                {
                    case WinApi.UOI.FLAGS:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        return (ptr.ToInt32().ToString(), Marshal.GetLastWin32Error());
                    case WinApi.UOI.NAME:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        var name = Marshal.PtrToStringAnsi(ptr);
                        return (name, Marshal.GetLastWin32Error());
                    case WinApi.UOI.TYPE:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        var type = Marshal.PtrToStringAnsi(ptr);
                        return (type, Marshal.GetLastWin32Error());
                    case WinApi.UOI.USER_SID:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        return (ptr.ToInt64().ToString(), Marshal.GetLastWin32Error());
                    case WinApi.UOI.HEAPSIZE:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        return (ptr.ToInt64().ToString(), Marshal.GetLastWin32Error());
                    case WinApi.UOI.IO:
                        WinApi.GetUserObjectInformation(winsta, (int)uoi, IntPtr.Zero, 0, out needed);
                        ptr = Marshal.AllocHGlobal(needed);
                        if (!WinApi.GetUserObjectInformation(winsta, (int)uoi, ptr, needed, out needed)) return ("Error", needed);

                        var io = Marshal.PtrToStringAnsi(ptr);
                        return (io, Marshal.GetLastWin32Error());
                    default:
                        return ("none", int.MinValue);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static IEnumerable<(int, string)> WTSEnumerateSessions()
        {
            WinApi.WTSEnumerateSessions(IntPtr.Zero, 0, 1, out var ppSessionInfo, out var pCount);

            var size = Marshal.SizeOf<WinApi.WTS_SESSION_INFO>();
            var current = (int)ppSessionInfo;

            for (var i = 0; i < pCount; i++)
            {
                var info = Marshal.PtrToStructure<WinApi.WTS_SESSION_INFO>((IntPtr)current);
                current += size;

                yield return (info.SessionId, info.WinStationName);
            }
            WinApi.WTSFreeMemory(ppSessionInfo);
        }
        public static IEnumerable<(int, int, string)> WTSEnumerateProcesses()
        {
            WinApi.WTSEnumerateProcesses(IntPtr.Zero, 0, 1, out var ppProcessInfo, out var pCount);

            var size = Marshal.SizeOf<WinApi.WTS_PROCESS_INFO>();
            var current = (int)ppProcessInfo;

            for (var i = 0; i < pCount; i++)
            {
                var info = Marshal.PtrToStructure<WinApi.WTS_PROCESS_INFO>((IntPtr)current);
                current += size;

                yield return (info.SessionId, info.ProcessId, info.ProcessName);
            }
            WinApi.WTSFreeMemory(ppProcessInfo);
        }
        public static int WTSSendMessage(int sessionId, string text, string title)
        {
            WinApi.WTSSendMessage(IntPtr.Zero, sessionId, title, title.Length, text, text.Length,
                (uint)(WinApi.MB.OK | WinApi.MB.ICONINFORMATION | WinApi.MB.TOPMOST | WinApi.MB.SETFOREGROUND | WinApi.MB.DEFAULT_DESKTOP_ONLY | WinApi.MB.SERVICE_NOTIFICATION),
                0, out var response, false);
            return Marshal.GetLastWin32Error();
        }
        public static IEnumerable<(int, string, int)> WTSSendMessageToAllSessions(string text, string title)
        {
            foreach(var (id, name) in WTSEnumerateSessions())
            {
                var code = WTSSendMessage(id, text, title);
                yield return (id, name, code);
            }
        }
        public static int WTSTerminateProcess(int pid)
        {
            WinApi.WTSTerminateProcess(IntPtr.Zero, pid, 0);
            return Marshal.GetLastWin32Error();
        }

        public static IEnumerable<(IntPtr, string)> EnumWindowStations()
        {
            var list = new List<string>();
            var gch = GCHandle.Alloc(list);
            var childProc = new WinApi.EnumWindowStationsDelegate(WinApi.EnumWindowStationsCallback);

            WinApi.EnumWindowStations(childProc, GCHandle.ToIntPtr(gch));

            foreach (var winSta in list)
            {
                yield return (WinApi.OpenWindowStation(winSta, false, (uint)WinApi.WINSTA.ALL_ACCESS), winSta);
            }
        }

        public static IEnumerable<(IntPtr, string)> EnumDesktops(IntPtr hWinSta)
        {
            var list = new List<string>();
            var gch = GCHandle.Alloc(list);
            var child = new WinApi.EnumDesktopsDelegate(WinApi.EnumDesktopsCallback);

            WinApi.EnumDesktops(hWinSta, child, GCHandle.ToIntPtr(gch));

            foreach (var desktop in list)
            {
                yield return (WinApi.OpenDesktop(desktop, 0, false, (uint)WinApi.DESKTOP.ALL), desktop);
            }
        }

        public static IEnumerable<IntPtr> EnumDesktopWindows(IntPtr hDesktop)
        {
            var list = new List<IntPtr>();
            var gch = GCHandle.Alloc(list);
            var child = new WinApi.EnumWindowsDelegate(WinApi.EnumWindowsCallback);

            WinApi.EnumDesktopWindows(hDesktop, child, GCHandle.ToIntPtr(gch));

            foreach (var desktopWindow in list)
            {
                yield return desktopWindow;
            }
        }
    }
}
