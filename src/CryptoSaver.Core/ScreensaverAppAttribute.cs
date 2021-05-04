using System;
using System.Windows;

namespace CryptoSaver.Core
{
    /// <summary>
    /// Class ScreensaverAppAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScreensaverAppAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreensaverAppAttribute"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <exception cref="ArgumentException">Type does not derive from Window</exception>
        public ScreensaverAppAttribute(Type mainWindow)
        {
            if (mainWindow == null) throw new ArgumentException("Type does not derive from Window", nameof(mainWindow));
            if (mainWindow.BaseType == typeof(Window))
                MainWindow = (Window)Activator.CreateInstance(mainWindow);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreensaverAppAttribute"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="configWindow">The configuration window.</param>
        /// <exception cref="ArgumentException">Type does not derive from Window</exception>
        public ScreensaverAppAttribute(Type mainWindow, Type configWindow) : this(mainWindow)
        {
            if (mainWindow == null)
                throw new ArgumentException("Type does not derive from Window", nameof(configWindow));
            if (configWindow.BaseType == typeof(Window))
                ConfigWindow = (Window)Activator.CreateInstance(configWindow);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [stretch across monitors].
        /// </summary>
        /// <value><c>true</c> if [stretch across monitors]; otherwise, <c>false</c>.</value>
        public bool StretchAcrossMonitors { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to [repeat on each monitor].
        /// </summary>
        /// <value><c>true</c> if [repeat on each monitor]; otherwise, <c>false</c>.</value>
        public bool RepeatOnEachMonitor { get; set; }
        /// <summary>
        /// Gets the main screensaver window.
        /// </summary>
        /// <value>The main window.</value>
        internal Window MainWindow { get; }
        /// <summary>
        /// Gets the screensaver configuration window.
        /// </summary>
        /// <value>The configuration window.</value>
        internal Window ConfigWindow { get; }

        /// <summary>
        /// Gets or sets the restart on unhandled.
        /// </summary>
        /// <value>The restart on unhandled.</value>
        public bool RestartOnUnhandled { get; set; }
    }
}