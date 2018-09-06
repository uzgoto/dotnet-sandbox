using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    public static class SafeUserApi
    {
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nMaxCount);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SendMessage(IntPtr hWnd, WM Msg, SC wParam, IntPtr lParam);

            #region Enum
            internal enum WM
            {
                CLOSE = 0x0002,
                SYSCOMMAND = 0x0112,
            }
            internal enum SC
            {
                CLOSE = 0xF060
            }
            #endregion
        }

        public static IEnumerable<IntPtr> EnumWindowHandles(IntPtr hWndRoot)
        {
            yield return hWndRoot;

            var child = IntPtr.Zero;
            while ((child = NativeMethods.FindWindowEx(hWndRoot, child, null, null)) != IntPtr.Zero)
            {
                foreach (var childchild in EnumWindowHandles(child))
                {
                    yield return childchild;
                }
            }
        }

        internal static Process GetProcess(IntPtr hWnd)
        {
            NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);
            return Process.GetProcessById(processId);
        }

        internal static string GetClassName(IntPtr hWnd)
        {
            var builder = new StringBuilder(256);
            NativeMethods.GetClassName(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        internal static string GetWindowText(IntPtr hWnd)
        {
            var builder = new StringBuilder(256);
            NativeMethods.GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        public static void Close(IntPtr hWnd)
        {
            SilentlyClose(hWnd);
            var code = Marshal.GetLastWin32Error();
            if (code != 0)
            {
                throw new Win32Exception(code);
            }
        }

        public static void SilentlyClose(IntPtr hWnd)
        {
            NativeMethods.SendMessage(hWnd, NativeMethods.WM.SYSCOMMAND, NativeMethods.SC.CLOSE, IntPtr.Zero);
        }

    }
}
