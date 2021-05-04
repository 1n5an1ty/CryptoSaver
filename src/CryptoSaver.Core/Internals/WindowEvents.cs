using System;
using System.Windows;
using System.Windows.Input;
using CryptoSaver.Core.Logging;

namespace CryptoSaver.Core.Internals
{
    internal static class WindowEvents
    {
        #region Static Fields and Constants

        private static Point _lastMousePoint = new Point(0, 0);
        private static readonly Type This = typeof(WindowEvents);
        #endregion

        internal static void Window_KeyDown(object sender, KeyEventArgs e)
        {
            This.Log().Info("Received key [" + e.Key + "] press");
            ShutDown(sender);
        }

        internal static void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            This.Log().Info("Received mouse click");
            ShutDown(sender);
        }

        internal static void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition((Window)sender);
            if (_lastMousePoint.X.Equals(0) && _lastMousePoint.Y.Equals(0))
                _lastMousePoint = currentPoint;

            else if (Math.Abs(currentPoint.X - _lastMousePoint.X) > 5 ||
                     Math.Abs(currentPoint.Y - _lastMousePoint.Y) > 5)
            {
                This.Log().Info("Received mouse movement");
                ShutDown(sender);
            }
            else
            {
                _lastMousePoint = currentPoint;
            }
        }

        internal static void Window_TouchDown(object sender, TouchEventArgs e)
        {
            This.Log().Info("Received screen touch");
            ShutDown(sender);
        }

        internal static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((Window)sender).WindowState = WindowState.Maximized;
        }

        internal static void Window_Closed(object sender, EventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private static void ShutDown(object sender)
        {
            var window = (Window)sender;
            This.Log().Debug("Shutting down " + window.Title);
            window.Close();
            Application.Current?.Shutdown();
        }
    }
}