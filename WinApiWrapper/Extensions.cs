using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Uzgoto.Dotnet.Sandbox.Winapi
{
    internal static class Extensions
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

            internal enum WM
            {
                CLOSE = 0x0002,
                SYSCOMMAND = 0x0112,
            }
            internal enum SC
            {
                CLOSE = 0xF060
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
        }
    }
}
