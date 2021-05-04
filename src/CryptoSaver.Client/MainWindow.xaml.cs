using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CryptoSaver.Core;
using CryptoSaver.Core.ExtensionMethods;
using CryptoSaver.Core.Logging;

namespace CryptoSaver.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Log().Info("My Screensaver is starting");
            var startPoint = 1;
            while (true)
            {
                if (startPoint == 1)
                    txtMessage.Text = "Application Started!";
                else
                    txtMessage.Text = DateTime.Now.ToString();

                //MoveText(startPoint * 1.33);
                await Task.Delay(TimeSpan.FromSeconds(2.33));
                startPoint++;
            }
        }

        private void MoveText(double start)
        {
            //txtMessage.SetValue(TextBlock., start);
        }
    }
}
