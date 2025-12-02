using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace Pipboy.Wallpaper.Utils;

internal class WindowsUtils
{
    public static double GetTaskbarHeight()
    {
       var window = Application.Current.MainWindow;
       return GetTaskbarHeight(window);
    }
    public static double GetTaskbarHeight(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        var windowHandle = new WindowInteropHelper(window).Handle;
        var screen = Screen.FromHandle(windowHandle);

        var screenBounds = screen.Bounds;
        var workingArea = screen.WorkingArea;

        int taskbarHeight = screenBounds.Height - workingArea.Height;
        int taskbarWidth = screenBounds.Width - workingArea.Width;

        if (workingArea.Top > screenBounds.Top)
        {
            return workingArea.Top - screenBounds.Top;
        }
        else if (workingArea.Bottom < screenBounds.Bottom)
        {
            return screenBounds.Bottom - workingArea.Bottom;
        }
        return 0;
    }
}