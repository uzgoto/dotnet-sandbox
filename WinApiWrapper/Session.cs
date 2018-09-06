using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public static class Session
    {
        private static class NativeMethods
        {
            public enum WTS_INFO_CLASS
            {
                WTSInitialProgram,
                WTSApplicationName,
                WTSWorkingDirectory,
                WTSOEMId,
                WTSSessionId,
                WTSUserName,
                WTSWinStationName,
                WTSDomainName,
                WTSConnectState,
                WTSClientBuildNumber,
                WTSClientName,
                WTSClientDirectory,
                WTSClientProductId,
                WTSClientHardwareId,
                WTSClientAddress,
                WTSClientDisplay,
                WTSClientProtocolType,
                WTSIdleTime,
                WTSLogonTime,
                WTSIncomingBytes,
                WTSOutgoingBytes,
                WTSIncomingFrames,
                WTSOutgoingFrames,
                WTSClientInfo,
                WTSSessionInfo
            }

            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern uint WTSGetActiveConsoleSessionId();
            [DllImport("Wtsapi32.dll")]
            internal static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
            [DllImport("Wtsapi32.dll")]
            internal static extern void WTSFreeMemory(IntPtr pointer);
        }

        public class User
        {
            public string Domain { get; internal set; }
            public string Name { get; internal set; }
        }

        public static int GetCurrentSessionId() => (int)NativeMethods.WTSGetActiveConsoleSessionId();

        public static User GetUser(int sessionId)
        {
            return new User()
            {
                Domain = GetSessionInfo(sessionId, NativeMethods.WTS_INFO_CLASS.WTSDomainName),
                Name = GetSessionInfo(sessionId, NativeMethods.WTS_INFO_CLASS.WTSUserName),
            };
        }

        private static string GetSessionInfo(int sessionId, NativeMethods.WTS_INFO_CLASS type)
        {
            if(NativeMethods.WTSQuerySessionInformation(
                IntPtr.Zero, sessionId, type,
                out var buffer, out var size))
            {
                if(size > 1)
                {
                    var value = Marshal.PtrToStringAnsi(buffer);
                    NativeMethods.WTSFreeMemory(buffer);
                    return value;
                }
            }
            return string.Empty;
        }

        
    }
}
