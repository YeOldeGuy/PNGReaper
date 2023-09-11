using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MahApps.Metro.Controls;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace PNGReaper.Helpers;

public class RoundedMetroWindow : MetroWindow
{
    // The DWM_WINDOW_CORNER_PREFERENCE enum for
    // DwmSetWindowAttribute's third parameter, which tells the
    // function what value of the enum to set. Copied from dwmapi.h

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT    = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND      = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // The enum flag for DwmSetWindowAttribute's second parameter, which
    // tells the function what attribute to set. Copied from dwmapi.h
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    protected void SetRoundedCorners()
    {
        var hWnd = new WindowInteropHelper(
            GetWindow(this) ?? throw new InvalidOperationException()).EnsureHandle();
        const DWMWINDOWATTRIBUTE attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute
    // in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern void DwmSetWindowAttribute(IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
        uint cbAttribute);
}