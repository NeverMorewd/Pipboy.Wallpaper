using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Utils;

internal static class WindowsUtils
{
    public static double GetTaskbarThickness(Window? window = null)
    {
        Screen screen;

        if (window != null)
        {
            var windowHandle = new WindowInteropHelper(window).Handle;
            if (windowHandle != IntPtr.Zero)
            {
                screen = Screen.FromHandle(windowHandle);
            }
            else
            {
                // Handle not yet created, fall back to primary
                screen = Screen.PrimaryScreen ?? throw new InvalidOperationException("No primary screen available.");
            }
        }
        else
        {
            // Use primary screen when window is null or not loaded
            screen = Screen.PrimaryScreen ?? throw new InvalidOperationException("No primary screen available.");
        }

        var screenBounds = screen.Bounds;
        var workingArea = screen.WorkingArea;

        int thickness = 0;

        if (workingArea.Top > screenBounds.Top)
        {
            thickness = workingArea.Top - screenBounds.Top; // top
        }
        else if (workingArea.Bottom < screenBounds.Bottom)
        {
            thickness = screenBounds.Bottom - workingArea.Bottom; // bottom
        }
        else if (workingArea.Left > screenBounds.Left)
        {
            thickness = workingArea.Left - screenBounds.Left; // left
        }
        else if (workingArea.Right < screenBounds.Right)
        {
            thickness = screenBounds.Right - workingArea.Right; // right
        }
        double dpiScaleY = 1.0;
        if (window != null)
        {
            dpiScaleY = VisualTreeHelper.GetDpi(window).DpiScaleY;
        }
        else
        {
            //ignore 
        }

        return thickness / dpiScaleY;
    }
}