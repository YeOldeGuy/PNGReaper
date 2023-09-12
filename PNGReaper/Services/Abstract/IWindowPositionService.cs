using System.Windows;
using PNGReaper.Helpers;

namespace PNGReaper.Services.Abstract;

public interface IWindowPositionService
{
    void SetPosition(Window window, WindowPlacement placement);
    WindowPlacement GetPosition(Window window);
}