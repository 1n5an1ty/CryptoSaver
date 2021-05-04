using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using CryptoSaver.Core.ExtensionMethods;
using CryptoSaver.Core.Internals;
using CryptoSaver.Core.Logging;
using Brushes = System.Windows.Media.Brushes;

namespace CryptoSaver.Core.ExtensionMethods
{
    /// <summary>
    /// Class ScreenSaverWindowExtensions used to show a WPF window as a screensaver.
    /// </summary>
    public static class ScreenSaverWindowExtensions
    {
        private static readonly Type This = typeof(ScreenSaverWindowExtensions);

        internal static void ShowPreview(this Window window, IntPtr intPtr)
        {
            This.Log().Debug("Showing Preview");
            window.Closed += WindowEvents.Window_Closed;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.WindowStyle = WindowStyle.None;
            window.WindowState = WindowState.Normal;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;

            var helper = new WindowInteropHelper(window) { Owner = intPtr };

            // Get size on Parent
            var rect = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(intPtr, ref rect);
            window.Top = 0;
            window.Left = 0;
            window.Width = rect.Right - rect.Left;
            window.Height = rect.Bottom - rect.Top;
            window.Show();

            // Set the preview window as the parent of this window
            NativeMethods.SetParent(helper.Handle, intPtr);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            NativeMethods.SetWindowLong(helper.Handle, -16,
                new IntPtr(NativeMethods.GetWindowLong(helper.Handle, -16) | 0x40000000));
        }

        internal static void Show(this Window window, Screen screen)
        {
            This.Log().Debug("Showing " + window.Title + " on " + screen.DeviceName);
            Rectangle bounds = screen.Bounds;
            window.Top = bounds.Top;
            window.Left = bounds.Left;
            window.Width = bounds.Width;
            window.Height = bounds.Height;

            window.Show();
        }

        internal static void ShowConfig(this Window window)
        {
            This.Log().Debug("Showing Config");
            window.Closed += WindowEvents.Window_Closed;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        /// <summary>
        /// Shows the screen saver.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="stretch">if set to <c>true</c> [stretch].</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public static void ShowScreenSaver(this Window window, bool stretch = false, bool repeat = false)
        {
            double top = Screen.AllScreens.Min(screen => screen.Bounds.Top);
            double left = Screen.AllScreens.Min(screen => screen.Bounds.Left);
            double width = Screen.AllScreens.Sum(screen => screen.Bounds.Width);
            double height = Screen.AllScreens.Sum(screen => screen.Bounds.Height);

            if (stretch && width <= window.MaxWidth && height <= window.MaxHeight)
            {
                This.Log().Debug("Stretching Display across monitors");
                window.SetWindowStyles();
                window.SetupWindowEvents();

                window.Top = top;
                window.Left = left;
                window.Width = width;
                window.Height = height;
                window.Show();
            }
            else
            {
                foreach (var s in Screen.AllScreens)
                {
                    if (!repeat)
                    {
                        if (Equals(s, Screen.PrimaryScreen))
                        {
                            window.SetWindowStyles();
                            window.SetupWindowEvents();
                            window.Show(s);
                        }
                        else
                            ShadeWindow.Show(s);
                    }
                    else
                    {
                        This.Log().Debug("Repeating screensaver on " + s.DeviceName);
                        Window newWindow = (Window)Activator.CreateInstance(window.GetType());
                        newWindow.SetWindowStyles();
                        newWindow.SetupWindowEvents();
                        newWindow.Show(s);
                    }
                }
            }
        }



        private static Window _shadeWindow;
        private static Window ShadeWindow
        {
            get
            {
                if (_shadeWindow != null) return _shadeWindow;
                var window = new Window { Background = Brushes.Black };
                window.SetupWindowEvents();
                window.SetWindowStyles();

                window.WindowState = WindowState.Normal;
                _shadeWindow = window;
                return _shadeWindow;
            }
        }

        /// <summary>
        /// Sets the window styles approbate for a screen saver and by release/build.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void SetWindowStyles(this Window window)
        {
            if (Debugger.IsAttached)
            {
                This.Log().Debug("Setting debug styles to " + window.Title);
                window.ShowInTaskbar = true;
                window.Topmost = false;
            }
            else
            {
                This.Log().Debug("Setting Release styles to " + window.Title);
                window.WindowStyle = WindowStyle.None;
                window.Cursor = System.Windows.Input.Cursors.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.ShowInTaskbar = false;
                window.Topmost = true;
            }
        }

        private static void SetupWindowEvents(this Window window)
        {
            This.Log().Debug("Setting up events on " + window.Title);
            Thread.Sleep(600);
            window.Closed += WindowEvents.Window_Closed;
            window.MouseDown += WindowEvents.Window_MouseDown;
            window.TouchDown += WindowEvents.Window_TouchDown;
            window.KeyDown += WindowEvents.Window_KeyDown;
            window.MouseMove += WindowEvents.Window_MouseMove;
        }
    }
}