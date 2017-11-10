using System;
using System.Runtime.InteropServices;

namespace RemoteConnectionManager.Core.Interop
{
    public class WindowsInterop
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(
            IntPtr hWnd,
            int nIndex,
            int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x, int y,
            int cx, int cy,
            uint uFlags);
        
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOACTIVATE = 0x0010U;
        public const uint SWP_SHOWWINDOW = 0x0040U;
        public const uint SWP = SWP_NOACTIVATE | SWP_SHOWWINDOW;
    }
}
