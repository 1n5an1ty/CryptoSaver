using CryptoSaver.Core;
using CryptoSaver.Client.Properties;
using System.Windows;
using System;

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
            var threadCount = (int)(Environment.ProcessorCount * Settings.Default.Threads);
            Operations.SetupMinerProperties(Settings.Default.StratumURL, Settings.Default.Username, Settings.Default.Password, threadCount);
            Operations.App_Startup(sender, e);
        }

        private void Core_UnhandledException(object sender,
            ref System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Your exception handling logic if you have resolved the exception
            // set e.Handled = true;
        }
    }
}
