using System;
using System.Diagnostics;
using System.Security.Principal;
using CryptoSaver.Core.Logging;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace CryptoSaver.Core.Internals
{
    /// <summary>
    /// Class Installation used to install screensaver.
    /// </summary>
    internal static class Installation
    {
        private static readonly Type This = typeof(Installation);
        #region Properties & Indexers

        /// <summary>
        /// Gets a value indicating whether this instance is admin.
        /// </summary>
        /// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
        internal static bool IsAdmin
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                if (identity == null) return true;

                if (new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    This.Log().Debug(identity.Name + " is an administrator");
                    return true;
                }
                This.Log().Info(identity.Name + " is not an administrator");
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Installs the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="launchWindowsConfig">if set to <c>true</c> [launch windows configuration].</param>
        public static void Install(string file, bool launchWindowsConfig = true)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                key.SetValue("ScreenSaveActive", 1, RegistryValueKind.String);
                key.SetValue("SCRNSAVE.EXE", file, RegistryValueKind.String);
                if (launchWindowsConfig) LaunchScreenSaverConfig();
            }
            catch (Exception e)
            {
                This.Log().Error("Error during install " + e.Message, e);
                Application.Current.Shutdown((int)ShutdownCodes.InstallationFailure);
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Launches the screen saver configuration.
        /// </summary>
        private static void LaunchScreenSaverConfig()
        {
            This.Log().Debug("Launching windows screensaver configuration");
            var process = new ProcessStartInfo(@"C:\Windows\System32\rundll32.exe", " shell32.dll,Control_RunDLL desk.cpl,,1") { UseShellExecute = true };
            Process.Start(process);
        }

        /// <summary>
        /// Restarts the elevated.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="args">The arguments.</param>
        internal static void RestartElevated(string file, string[] args)
        {
            This.Log().Debug("Restarting elevated");
            var elevated = new ProcessStartInfo(file, args[0] + " Elevated")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            Process.Start(elevated);
            Application.Current.Shutdown((int)ShutdownCodes.RestartingElevated);
        }
    }
}