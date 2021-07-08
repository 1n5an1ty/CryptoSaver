using CryptoSaver.Core;
using CryptoSaver.Client.Properties;
using System.Windows;
using System;
using CryptoSaver.Core.Miner;

namespace CryptoSaver.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [ScreensaverApp(typeof(MainWindow), typeof(ConfigWindow), RepeatOnEachMonitor = true)]
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Operations.UnhandledException += Core_UnhandledException;
            Operations.SetupMinerProperties(BuildMinerSettings());
            Operations.App_Startup(sender, e);
        }

        private MinerSettings BuildMinerSettings() =>
            new MinerSettings
            {
                StratumURL = Settings.Default.StratumURL,
                Username = Settings.Default.Username,
                Password = Settings.Default.Password,
                Threads = GetThreadCount(),
                EnableAMD = Settings.Default.EnableAMD,
                EnableNVIDIA = Settings.Default.EnableNVIDIA
            };

        private int GetThreadCount() =>
             (int)(Environment.ProcessorCount * Settings.Default.Threads);

        private void Core_UnhandledException(object sender,
            ref System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Your exception handling logic if you have resolved the exception
            // set e.Handled = true;
        }
    }
}
