using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

public class WindowPositionService : IWindowPositionService
{
    public void SetPosition(Window window, WindowPlacement placement)
    {
        WindowPInvokeMethods.SetWindowPlacement(window, placement);
    }

    public WindowPlacement GetPosition(Window window)
    {
        var p = WindowPInvokeMethods.GetWindowPlacement(window, out var pos) ? pos : new WindowPlacement();
        return p;
    }

    private static class WindowPInvokeMethods
    {
        public const int SwShowNormal    = 1;
        public const int SwShowMinimized = 2;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        public static bool SetWindowPlacement(Window window, WindowPlacement placement)
        {
            var handle = new WindowInteropHelper(window).Handle;
            return SetWindowPlacement(handle, ref placement);
        }

        public static bool GetWindowPlacement(Window window, out WindowPlacement placement)
        {
            var handle = new WindowInteropHelper(window).Handle;
            return GetWindowPlacement(handle, out placement);
        }
    }
}