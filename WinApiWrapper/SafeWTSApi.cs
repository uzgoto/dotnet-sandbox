using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public static class SafeWTSApi
    {
        private static class NativeMethods
        {
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern bool WTSSendMessage(
                IntPtr hServer,
                [MarshalAs(UnmanagedType.I4)]int sessionId,
                string pTitle,
                [MarshalAs(UnmanagedType.U4)]int titleLength,
                string pMessage,
                [MarshalAs(UnmanagedType.U4)]int messageLength,
                [MarshalAs(UnmanagedType.U4)]MB style,
                [MarshalAs(UnmanagedType.U4)]int timeout,
                [MarshalAs(UnmanagedType.U4)]out int pResponse,
                bool bWait);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSEnumerateSessions(
                IntPtr serverHandle, int reserved, int version, out IntPtr ppSessionInfo, out int pCount);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSEnumerateProcesses(
                IntPtr serverHandle, int reserved, int version, out IntPtr ppProcessInfo, out int pCount);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern void WTSFreeMemory(IntPtr memory);
            [DllImport("wtsapi32.dll", SetLastError = true)]
            internal static extern bool WTSTerminateProcess(IntPtr serverHande, int processId, int exitCode);

            #region Struct
            [StructLayout(LayoutKind.Sequential)]
            internal struct WTS_SESSION_INFO
            {
                public int SessionId { get; set; }
                public string WinStationName { get; set; }
                public WTS_CONNECTSTATE_CLASS State { get; set; }
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

            #region Enum
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
            #endregion
        }

        public class WTSSession
        {
            public int SessionId { get; set; }
            public string WindowStationName { get; set; }
        }
        public class WTSProcess
        {
            public int SessionId { get; set; }
            public int ProcessId { get; set; }
            public string ProcessName { get; set; }
            public string UserId { get; set; }
        }

        private static WTSSession WTSSessionFromNative(NativeMethods.WTS_SESSION_INFO info)
        {
            return new WTSSession()
            {
                SessionId = info.SessionId,
                WindowStationName = info.WinStationName,
            };
        }

        private static WTSProcess WTSProcessFromNative(NativeMethods.WTS_PROCESS_INFO info)
        {
            return new WTSProcess()
            {
                SessionId = info.SessionId,
                ProcessId = info.ProcessId,
                ProcessName = info.ProcessName,
                UserId = info.UserId,
            };
        }

        private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        public static IEnumerable<WTSSession> WTSEnumerateSessions()
        {
            NativeMethods.WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, out var ppSessionInfo, out var pCount);

            var size = Marshal.SizeOf<NativeMethods.WTS_SESSION_INFO>();
            var current = (int)ppSessionInfo;

            for (var i = 0; i < pCount; i++)
            {
                var info = Marshal.PtrToStructure<NativeMethods.WTS_SESSION_INFO>((IntPtr)current);
                current += size;

                yield return WTSSessionFromNative(info);
            }
            NativeMethods.WTSFreeMemory(ppSessionInfo);
        }

        public static IEnumerable<WTSProcess> WTSEnumerateProcesses()
        {
            NativeMethods.WTSEnumerateProcesses(WTS_CURRENT_SERVER_HANDLE, 0, 1, out var ppProcessInfo, out var pCount);

            var size = Marshal.SizeOf<NativeMethods.WTS_PROCESS_INFO>();
            var current = (int)ppProcessInfo;

            for (var i = 0; i < pCount; i++)
            {
                var info = Marshal.PtrToStructure<NativeMethods.WTS_PROCESS_INFO>((IntPtr)current);
                current += size;

                yield return WTSProcessFromNative(info);
            }
            NativeMethods.WTSFreeMemory(ppProcessInfo);
        }

        public static IEnumerable<WTSProcess> WTSEnumerateProcesses(int sessionId)
        {
            foreach (var process in WTSEnumerateProcesses())
            {
                if(process.SessionId == sessionId)
                {
                    yield return process;
                }
            }
        }

        public static WTSProcess GetWTSProcessId(int sessionId, string processName)
        {
            foreach (var process in WTSEnumerateProcesses())
            {
                if (process.SessionId == sessionId && process.ProcessName == processName)
                {
                    return process;
                }
            }
            return null;
        }

        public static int WTSSendMessageInformation(int sessionId, string text, string title)
        {
            NativeMethods.WTSSendMessage(IntPtr.Zero, sessionId, title, title.Length, text, text.Length,
                NativeMethods.MB.OK | NativeMethods.MB.ICONINFORMATION | NativeMethods.MB.TOPMOST | NativeMethods.MB.SETFOREGROUND | NativeMethods.MB.DEFAULT_DESKTOP_ONLY | NativeMethods.MB.SERVICE_NOTIFICATION,
                0, out var response, false);
            return Marshal.GetLastWin32Error();
        }

        public static int WTSSendMessageWarning(int sessionId, string text, string title)
        {
            NativeMethods.WTSSendMessage(IntPtr.Zero, sessionId, title, title.Length, text, text.Length,
                NativeMethods.MB.OK | NativeMethods.MB.ICONWARNING | NativeMethods.MB.TOPMOST | NativeMethods.MB.SETFOREGROUND | NativeMethods.MB.DEFAULT_DESKTOP_ONLY | NativeMethods.MB.SERVICE_NOTIFICATION,
                0, out var response, false);
            return Marshal.GetLastWin32Error();
        }

        public static IEnumerable<int> WTSSendMessageToAllSessions(string text, string title)
        {
            foreach (var wtsSession in WTSEnumerateSessions())
            {
                var code = WTSSendMessageInformation(wtsSession.SessionId, text, title);
                yield return code;
            }
        }

        public static int WTSTerminateProcess(int pid)
        {
            NativeMethods.WTSTerminateProcess(WTS_CURRENT_SERVER_HANDLE, pid, 0);
            return Marshal.GetLastWin32Error();
        }
    }
}
