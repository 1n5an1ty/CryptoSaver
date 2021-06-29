using CryptoSaver.Core;
using System.Windows;

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
