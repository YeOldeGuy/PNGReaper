using System;
using System.Runtime.InteropServices;

namespace PNGReaper.Helpers;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rect(int left, int top, int right, int bottom)
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }
}

/// <summary>
///     Information pertaining to placement of a window. Includes
///     data about minimized/maximized. This app doesn't use
///     this information in any way, other than to pass it as a
///     parameter to methods defined over in WindowPInvokeMethods.cs
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct WindowPlacement
{
    public int   length; // length of this struct (for Win32 calls = 44)
    public int   flags;
    public int   showCmd;
    public Point minPosition;
    public Point maxPosition;
    public Rect  normalPosition;
}