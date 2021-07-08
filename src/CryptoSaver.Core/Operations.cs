using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using CryptoSaver.Core.ExtensionMethods;
using CryptoSaver.Core.Internals;
using CryptoSaver.Core.Logging;
using CryptoSaver.Core.Miner;
using Microsoft.Win32;

namespace CryptoSaver.Core
{
    /// <summary>
    /// Core ScreenSaver helper
    /// </summary>
    public static class Operations
    {
        private static readonly Type This = typeof(Operations);
        private static ScreensaverAppAttribute _screensaverAppAttribute;
        private static MinerSettings _minerSettings;

        public static void SetupMinerProperties(MinerSettings minerSettings)
        {
            _minerSettings = minerSettings;
        }

        /// <summary>
        /// The current run mode for the screen saver
        /// </summary>
        public static Mode CurrentMode;

        /// <summary>
        /// Delegate UnhandledExceptionRaisedHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        public delegate void UnhandledExceptionRaisedHandler(object sender, ref DispatcherUnhandledExceptionEventArgs e);
        /// <summary>
        /// Occurs when [unhandled exception].
        /// </summary>
        public static event UnhandledExceptionRaisedHandler UnhandledException;

        #region Mode enum

        /// <summary>
        /// Enum Mode
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown,
            /// <summary>
            /// Screen saver started in preview mode
            /// </summary>
            Preview,
            /// <summary>
            ///  Screen saver started in configuration mode 
            /// </summary>
            Config,
            /// <summary>
            ///  Screen saver started in show mode
            /// </summary>
            Show,
            /// <summary>
            ///  Screen saver started in install mode
            /// </summary>
            Install
        }

        #endregion

        internal static Mode GetMode(StartupEventArgs e)
        {
            CurrentMode = GetMode(e.Args);
            return CurrentMode;
        }

        private static Mode GetMode(string[] args)
        {
            if (args == null || !args.Any()) return Mode.Config;
            var arg = args[0].ToLower();
            switch (arg)
            {
                case "/c":
                    return Mode.Config;
                case "/p":
                    return Mode.Preview;
                case "/i":
                    return Mode.Install;
                case "/s":
                    return Mode.Show;
                default:
                    return Mode.Unknown;
            }
        }

        private static string GetCurrentModeString()
        {
            switch (CurrentMode)
            {
                case Mode.Preview:
                    return "/p";
                case Mode.Config:
                    return "/c";
                case Mode.Show:
                    return "/s";
                case Mode.Install:
                    return "/i Elevated";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Should be passed from the app.xml.cs App_Startup method
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="StartupEventArgs" /> instance containing the event data.</param>
        public static void App_Startup(object sender, StartupEventArgs e)
        {
            var app = (Application)sender;
            This.Log().Debug("Sarting up " + AssemblyInfo.ProductName());
            app.Dispatcher.UnhandledException += App_DispatcherUnhandledException;
            app.Exit += App_Exit;

            var screenSaverAtt = GetAttribute(sender);
            _screensaverAppAttribute = screenSaverAtt;
            var mode = GetMode(e);

            This.Log().Debug(mode + " mode recognized");
            switch (mode)
            {
                case Mode.Unknown:
                case Mode.Config:
                    if (screenSaverAtt?.ConfigWindow != null)
                    {
                        screenSaverAtt.ConfigWindow?.ShowConfig();
                    }
                    else
                    {
                        This.Log().Error("No Config Window configured");
                        Application.Current.Shutdown((int)ShutdownCodes.NoConfigWindow);
                    }
                    break;

                case Mode.Preview:
                    if (e.Args.Length > 1)
                    {
                        screenSaverAtt?.MainWindow?.ShowPreview(new IntPtr(long.Parse(e.Args[1])));
                        break;
                    }

                    This.Log().Error("Unknown or missing argument for switch " + e.Args[0]);
                    Application.Current.Shutdown((int)ShutdownCodes.UnknownArgument);
                    break;

                case Mode.Show:
                    // Show Main
                    screenSaverAtt?.MainWindow?.ShowScreenSaver(screenSaverAtt.StretchAcrossMonitors, screenSaverAtt.RepeatOnEachMonitor);
                    MinerOperations.StartMiner(_minerSettings);
                    break;

                case Mode.Install:
                    // Install 
                    var file = FileName.RenameToScr();
                    if (!Installation.IsAdmin && e.Args.Length == 1)
                        Installation.RestartElevated(file, e.Args);
                    else
                        Installation.Install(file);
                    break;
            }
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            sender.Log().Info("Shutting down " + AssemblyInfo.ProductName());
            MinerOperations.StopMiner();
        }

        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            sender.LogP().Fatal(sender, e.Exception);

            if (_screensaverAppAttribute != null && _screensaverAppAttribute.RestartOnUnhandled)
            {
                e.Handled = true;

                if (DateTime.Now.Subtract(GetRestartInfo()).Seconds < 15)
                {
                    This.Log()
                        .Warn("Application was restarted due to an error recently. Canceling reboot and shutting down instead.");
                    Application.Current.Shutdown((int)ShutdownCodes.CoolDown);
                    return;
                }

                This.Log().Info("Auto restart defined, attempting now.");


                if (CurrentMode == Mode.Config)
                {
                    var result = MessageBox.Show(
                        "The following error has occured, " + e.Exception.Message + ",\n\n" +
                        AssemblyInfo.ProductName() + " will restart.", "Fatal Error",
                        MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    if (result != MessageBoxResult.OK)
                    {
                        This.Log().Info("User declined restart");
                        Application.Current.Shutdown();
                        return;
                    }
                }

                SetRestartInfo(DateTime.Now);
                This.Log().Info("User allowed restart");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location, GetCurrentModeString());
                Application.Current.Shutdown();
                return;
            }
            else
            {
                if (UnhandledException != null)
                {
                    This.Log().Info("Broadcasting exception to listeners.");
                    UnhandledException?.Invoke(sender, ref e);
                    if (!e.Handled)
                    {
                        This.Log().Error("Exception not marked as handled by caller, shutting down");
                        Application.Current.Shutdown((int)ShutdownCodes.UnknownError);
                        return;
                    }
                }
                else
                {
                    e.Handled = true;
                    Application.Current.Shutdown((int)ShutdownCodes.UnknownError);
                    return;
                }
            }
        }

        private static void SetRestartInfo(DateTime dateTime)
        {

            var company = AssemblyInfo.CompanyName();
            var product = AssemblyInfo.ProductName();
            var subKey = "Software\\" + company + "\\" + product;
            var key = Registry.CurrentUser.CreateSubKey(subKey);

            key.SetValue("LastReboot", dateTime);
        }

        private static DateTime GetRestartInfo()
        {
            var key = Registry.CurrentUser.OpenSubKey(
                "Software\\" + AssemblyInfo.CompanyName() + "\\" +
                AssemblyInfo.ProductName(), false);

            try
            {
                return DateTime.Parse(key.GetValue("LastReboot").ToString());
            }
            catch (Exception)
            {
                SetRestartInfo(DateTime.MinValue);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the <see cref="ScreensaverAppAttribute"/> attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns>ScreenSaver.Helper.ScreensaverAppAttribute.</returns>
        private static ScreensaverAppAttribute GetAttribute<T>(T t)
        {
            // Get instance of the attribute.
            var myAttribute = t.GetType().GetCustomAttribute(typeof(ScreensaverAppAttribute)) as ScreensaverAppAttribute;
            if (myAttribute == null)
            {
                This.Log().Error("Could not find implementation of " + nameof(ScreensaverAppAttribute) + " make sure to add the " + nameof(ScreensaverAppAttribute) + " to your App.xaml.cs");
                throw new Exception("Could not find implementation of " + nameof(ScreensaverAppAttribute));
            }
            else
            {
                This.Log().Debug(nameof(ScreensaverAppAttribute) + " found, " + nameof(myAttribute.MainWindow) + " : " + nameof(myAttribute.MainWindow) + "  " + nameof(myAttribute.ConfigWindow) + " : " + nameof(myAttribute.ConfigWindow));
                // Get the Name value.
                Console.WriteLine("The Name Attribute is: {0}.", myAttribute.MainWindow);
                // Get the Level value.
                Console.WriteLine("The Level Attribute is: {0}.", myAttribute.ConfigWindow);
                return myAttribute;
            }
        }
    }
}