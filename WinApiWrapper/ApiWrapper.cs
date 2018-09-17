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
        private static class NativeMethods
        {
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

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool EnumWindowStations(EnumWindowStationsDelegate lpEnumFunc, IntPtr lParam);
            internal delegate bool EnumWindowStationsDelegate(string windowStation, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool EnumDesktops(IntPtr hWinSta, EnumDesktopsDelegate lpEnumFunc, IntPtr lParam);
            internal delegate bool EnumDesktopsDelegate(string lpszDesktop, IntPtr lParam);

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

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

            [Flags]
            internal enum Token : uint
            {
                AssignPrimary = 0x0001,
                Duplicate = 0x0002,
                Impersonate = 0x0004,
                Query = 0x0008,
                QuerySource = 0x0010,
                AdjustPrivileges = 0x0020,
                AdjustGroups = 0x0040,
                AdjustDefault = 0x0080,
                AdjustSessionid = 0x0100,
                Read = 0x00020000 | Query,
                AllAccess = 0x000F0000 | AssignPrimary | Duplicate | Impersonate | Query | QuerySource | AdjustPrivileges | AdjustGroups | AdjustDefault | AdjustSessionid,
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool DuplicateTokenEx(
                IntPtr hExistingToken,
                uint dwDesiredAccess,
                ref SecutiryAttributes lpTokenAttributes,
                SecurityImpersonationLevel ImpersonationLevel,
                TokenType TokenType,
                out IntPtr phNewToken);

            [StructLayout(LayoutKind.Sequential)]
            internal struct SecutiryAttributes
            {
                internal int nLength;
                internal IntPtr lpSecurityDescripter;
                internal int bInheritHandle;
            }
            
            internal enum SecurityImpersonationLevel
            {
                Anonymous,
                Identification,
                Impersonation,
                Delegation,
            }
            internal enum TokenType
            {
                Primary = 1,
                Impersonation,
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out Luid lpLuid);

            [StructLayout(LayoutKind.Sequential)]
            internal struct Luid
            {
                internal uint LowPart;
                internal int HighPart;
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool AdjustTokenPrivileges(
                IntPtr TokenHandle,
                bool DisableAllPrivileges,
                ref TOKEN_PRIVILEGES NewState,
                uint Zero,
                IntPtr Null1,
                IntPtr Null2);

            internal struct TOKEN_PRIVILEGES
            {
                public int PrivilegeCount;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
                public LUID_AND_ATTRIBUTES[] Privileges;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            internal struct LUID_AND_ATTRIBUTES
            {
                public Luid Luid;
                public uint Attributes;
            }

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern bool CreateProcessAsUser(
                IntPtr hToken,
                string lpApplicationName,
                string lpCommandLine,
                ref SecutiryAttributes lpProcessAttributes,
                ref SecutiryAttributes lpThreadAttributes,
                bool bInheritHandles,
                uint dwCreationFlags,
                IntPtr lpEnvironment,
                string lpCurrentDirectory,
                ref STARTUPINFO lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);

            [StructLayout(LayoutKind.Sequential)]
            internal struct STARTUPINFO
            {
                internal int cb;
                internal string lpReserved;
                internal string lpDesktop;
                internal string lpTitle;
                internal int dwX;
                internal int dwY;
                internal int dwXSize;
                internal int dwYSize;
                internal int dwXCountChars;
                internal int dwYCountChars;
                internal int dwFillAttribute;
                internal int dwFlags;
                internal short wShowWindow;
                internal short cbReserved2;
                internal int lpReserve2;
                internal int hStdInput;
                internal int hStdOutput;
                internal int hStdError;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct PROCESS_INFORMATION
            {
                internal IntPtr hProcess;
                internal IntPtr hThread;
                internal int dwProcessId;
                internal int dwThreadId;
            }
            [Flags]
            internal enum CreateProcessFlags : uint
            {
                DEBUG_PROCESS = 0x00000001,
                DEBUG_ONLY_THIS_PROCESS = 0x00000002,
                CREATE_SUSPENDED = 0x00000004,
                DETACHED_PROCESS = 0x00000008,
                CREATE_NEW_CONSOLE = 0x00000010,
                NORMAL_PRIORITY_CLASS = 0x00000020,
                IDLE_PRIORITY_CLASS = 0x00000040,
                HIGH_PRIORITY_CLASS = 0x00000080,
                REALTIME_PRIORITY_CLASS = 0x00000100,
                CREATE_NEW_PROCESS_GROUP = 0x00000200,
                CREATE_UNICODE_ENVIRONMENT = 0x00000400,
                CREATE_SEPARATE_WOW_VDM = 0x00000800,
                CREATE_SHARED_WOW_VDM = 0x00001000,
                CREATE_FORCEDOS = 0x00002000,
                BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
                ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
                INHERIT_PARENT_AFFINITY = 0x00010000,
                INHERIT_CALLER_PRIORITY = 0x00020000,
                CREATE_PROTECTED_PROCESS = 0x00040000,
                EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
                PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
                PROCESS_MODE_BACKGROUND_END = 0x00200000,
                CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
                CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
                CREATE_DEFAULT_ERROR_MODE = 0x04000000,
                CREATE_NO_WINDOW = 0x08000000,
                PROFILE_USER = 0x10000000,
                PROFILE_KERNEL = 0x20000000,
                PROFILE_SERVER = 0x40000000,
                CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
            }

            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern uint WTSGetActiveConsoleSessionId();
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            [Flags]
            public enum ACCESS_MASK : uint
            {
                DELETE = 0x00010000,
                READ_CONTROL = 0x00020000,
                WRITE_DAC = 0x00040000,
                WRITE_OWNER = 0x00080000,
                SYNCHRONIZE = 0x00100000,

                STANDARD_RIGHTS_REQUIRED = 0x000F0000,

                STANDARD_RIGHTS_READ = 0x00020000,
                STANDARD_RIGHTS_WRITE = 0x00020000,
                STANDARD_RIGHTS_EXECUTE = 0x00020000,

                STANDARD_RIGHTS_ALL = 0x001F0000,

                SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

                ACCESS_SYSTEM_SECURITY = 0x01000000,

                MAXIMUM_ALLOWED = 0x02000000,

                GENERIC_READ = 0x80000000,
                GENERIC_WRITE = 0x40000000,
                GENERIC_EXECUTE = 0x20000000,
                GENERIC_ALL = 0x10000000,

                DESKTOP_READOBJECTS = 0x00000001,
                DESKTOP_CREATEWINDOW = 0x00000002,
                DESKTOP_CREATEMENU = 0x00000004,
                DESKTOP_HOOKCONTROL = 0x00000008,
                DESKTOP_JOURNALRECORD = 0x00000010,
                DESKTOP_JOURNALPLAYBACK = 0x00000020,
                DESKTOP_ENUMERATE = 0x00000040,
                DESKTOP_WRITEOBJECTS = 0x00000080,
                DESKTOP_SWITCHDESKTOP = 0x00000100,

                WINSTA_ENUMDESKTOPS = 0x00000001,
                WINSTA_READATTRIBUTES = 0x00000002,
                WINSTA_ACCESSCLIPBOARD = 0x00000004,
                WINSTA_CREATEDESKTOP = 0x00000008,
                WINSTA_WRITEATTRIBUTES = 0x00000010,
                WINSTA_ACCESSGLOBALATOMS = 0x00000020,
                WINSTA_EXITWINDOWS = 0x00000040,
                WINSTA_ENUMERATE = 0x00000100,
                WINSTA_READSCREEN = 0x00000200,

                WINSTA_ALL_ACCESS = 0x0000037F
            }
        }

        public static bool CreateProcessAsUser(string applicationName, out Process proc)
        {
            var dwSessionId = NativeMethods.WTSGetActiveConsoleSessionId();
            var winlogonProcess = Process.GetProcessesByName("winlogon").FirstOrDefault(p => (uint)p.SessionId == dwSessionId);

            var hProcess = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.All, false, winlogonProcess.Id);
            if (!NativeMethods.OpenProcessToken(hProcess, (uint)NativeMethods.Token.Duplicate, out var hPToken))
            {
                //CloseHandle(hProcess);
                proc = null;
                return false;
            }
            var sa = new NativeMethods.SecutiryAttributes();
            sa.nLength = Marshal.SizeOf<NativeMethods.SecutiryAttributes>();
            if (!NativeMethods.DuplicateTokenEx(hPToken, (uint)NativeMethods.ACCESS_MASK.MAXIMUM_ALLOWED, ref sa, NativeMethods.SecurityImpersonationLevel.Identification, NativeMethods.TokenType.Primary, out var hUserTokenDup))
            {
                //CloseHandle(hProcess);
                //CloseHandle(hPToken);
                proc = null;
                return false;
            }
            var sInfo = new NativeMethods.STARTUPINFO();
            sInfo.cb = Marshal.SizeOf<NativeMethods.STARTUPINFO>();
            sInfo.lpDesktop = @"winsta0\default";
            if (NativeMethods.CreateProcessAsUser(
                    hUserTokenDup,
                    null,
                    "cmd.exe",
                    ref sa,
                    ref sa,
                    false,
                    (uint)(NativeMethods.CreateProcessFlags.NORMAL_PRIORITY_CLASS | NativeMethods.CreateProcessFlags.CREATE_NEW_CONSOLE),
                    IntPtr.Zero,
                    null,
                    ref sInfo,
                    out var procInfo))
            {
                proc = Process.GetProcessById(procInfo.dwProcessId);
                return true;
            }
            proc = null;
            return false;
        }

        public static int CreateProcessAsUser(string commandLine, string currentPath)
        {
            var dwSessionId = NativeMethods.WTSGetActiveConsoleSessionId();
            var winlogonProcess = Process.GetProcessesByName("winlogon").FirstOrDefault(p => p.SessionId == dwSessionId);

            if (!NativeMethods.OpenProcessToken(winlogonProcess.Handle, (uint)(NativeMethods.Token.Query | NativeMethods.Token.Impersonate | NativeMethods.Token.Duplicate), out var userToken))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var tokenAttributes = new NativeMethods.SecutiryAttributes();
            tokenAttributes.nLength = Marshal.SizeOf(tokenAttributes);

            if (!NativeMethods.DuplicateTokenEx(userToken, 0x10000000, ref tokenAttributes, NativeMethods.SecurityImpersonationLevel.Impersonation,
                NativeMethods.TokenType.Impersonation, out var newToken))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!NativeMethods.LookupPrivilegeValue(null, "SeDebugPrivilege", out var seDebugNameValue))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var tokPrivs = new NativeMethods.TOKEN_PRIVILEGES()
            {
                PrivilegeCount = 1,
                Privileges = new NativeMethods.LUID_AND_ATTRIBUTES[]
                {
                    new NativeMethods.LUID_AND_ATTRIBUTES() {Luid = seDebugNameValue, Attributes = 0x00000002 },
                },
            };
            if (!NativeMethods.AdjustTokenPrivileges(newToken, false, ref tokPrivs, 0, IntPtr.Zero, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var threadAttributes = new NativeMethods.SecutiryAttributes();
            threadAttributes.nLength = Marshal.SizeOf(threadAttributes);
            var pi = new NativeMethods.PROCESS_INFORMATION();
            var si = new NativeMethods.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "Winsta0\\Winlogon";
            // start the process using the new token
            if (!NativeMethods.CreateProcessAsUser(
                newToken,
                string.Empty, commandLine,
                ref tokenAttributes, ref threadAttributes, true,
                (uint)(NativeMethods.CreateProcessFlags.CREATE_NEW_CONSOLE | NativeMethods.CreateProcessFlags.INHERIT_CALLER_PRIORITY),
                IntPtr.Zero,
                currentPath, ref si, out pi))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return pi.dwProcessId;
        }

        public static IEnumerable<(IntPtr, string)> EnumWindowStations()
        {
            var list = new List<string>();
            NativeMethods.EnumWindowStations(Callback, GCHandle.ToIntPtr(GCHandle.Alloc(list)));
            foreach (var winSta in list)
            {
                yield return
                    (NativeMethods.OpenWindowStation(winSta, false, (uint)NativeMethods.WINSTA.ALL_ACCESS), winSta);
            }

            bool Callback(string winSta, IntPtr lParam)
            {
                if(GCHandle.FromIntPtr(lParam).Target is List<string> winStas)
                {
                    winStas.Add(winSta);
                    return true;
                }
                return false;
            }
        }

        public static IEnumerable<(IntPtr, string)> EnumDesktops(IntPtr hWinSta)
        {
            var list = new List<string>();
            NativeMethods.EnumDesktops(hWinSta, Callback, GCHandle.ToIntPtr(GCHandle.Alloc(list)));
            foreach (var desktop in list)
            {
                yield return
                    (NativeMethods.OpenDesktop(desktop, 0, false, (uint)NativeMethods.DESKTOP.ALL), desktop);
            }

            bool Callback(string desktop, IntPtr lParam)
            {
                if(GCHandle.FromIntPtr(lParam).Target is List<string> desktops)
                {
                    desktops.Add(desktop);
                    return true;
                }
                return false;
            }
        }

        public static IEnumerable<IntPtr> EnumDesktopWindows(IntPtr hDesktop)
        {
            var list = new List<IntPtr>();
            NativeMethods.EnumDesktopWindows(hDesktop, Callback, GCHandle.ToIntPtr(GCHandle.Alloc(list)));
            foreach (var desktopWindow in list)
            {
                yield return desktopWindow;
            }

            bool Callback(IntPtr hWnd, IntPtr lParam)
            {
                if(GCHandle.FromIntPtr(lParam).Target is List<IntPtr> callbackList)
                {
                    callbackList.Add(hWnd);
                    return true;
                }
                return false;
            }
        }
    }
}
