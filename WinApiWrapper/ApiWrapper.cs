using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    internal static class ApiWrapper
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

                ICONERROR = 0x0000_0010,
                ICONQUESTION = 0x0000_0020,
                ICONWARNING = 0x0000_0030,
                ICONINFORMATION = 0x0000_0040,

                SYSTEMMODAL = 0x0000_1000,
                TASKMODAL = 0x0000_2000,

                SETFOREGROUND = 0x0001_0000,
                DEFAULT_DESKTOP_ONLY = 0x0002_0000,
                TOPMOST = 0x0004_0000,
                SERVICE_NOTIFICATION = 0x0020_0000,
            }
        }

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

        internal static void ShowInformationDialog(string text, string caption)
        {
            var type =
                WinApi.MB.OK | WinApi.MB.ICONINFORMATION | 
                WinApi.MB.SETFOREGROUND | WinApi.MB.TOPMOST | WinApi.MB.SERVICE_NOTIFICATION;
            WinApi.MessageBox(IntPtr.Zero, text, caption, (uint)type);
            var code = Marshal.GetLastWin32Error();
            if(code != 0)
            {
                throw new Win32Exception(code);
            }
        }
    }
}
