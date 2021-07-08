using CryptoSaver.Client.Properties;
using System;
using System.Diagnostics;
using System.Windows;

namespace CryptoSaver.Client
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            txtStratum.Text = Settings.Default.StratumURL;
            txtUsername.Text = Settings.Default.Username;
            txtPassword.Text = Settings.Default.Password;
            sdrThreads.Value = Settings.Default.Threads;
            rbEnableAMD.IsChecked = Settings.Default.EnableAMD;
            rbEnableNVIDIA.IsChecked = Settings.Default.EnableNVIDIA;

            lblThreads.Content = $"Threads: {GetThreadPercentage()}";
        }

        private string GetThreadPercentage()
        {
            switch (sdrThreads.Value)
            {
                case 0.25:
                    return "25%";
                case 0.50:
                    return "50%";
                case 0.75:
                    return "75%";
                case 1:
                    return "100%";
                default:
                    return "";
            }
        }

        private void SaveChanges_Action(object sender, RoutedEventArgs e)
        {
            Settings.Default.StratumURL = txtStratum.Text;
            Settings.Default.Username = txtUsername.Text;
            Settings.Default.Password = txtPassword.Text;
            Settings.Default.Threads = sdrThreads.Value;
            Settings.Default.EnableAMD = rbEnableAMD.IsChecked ?? false;
            Settings.Default.EnableNVIDIA = rbEnableNVIDIA.IsChecked ?? false;


            Settings.Default.Save();

            Settings.Default.Reload();

            this.Close();
        }

        private void sdrThreads_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblThreads.Content = $"Threads: {GetThreadPercentage()}";
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
