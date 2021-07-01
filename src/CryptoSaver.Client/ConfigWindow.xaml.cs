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
            txtStratum.Text = Properties.Settings.Default.StratumURL;
            txtUsername.Text = Properties.Settings.Default.Username;
            txtPassword.Text = Properties.Settings.Default.Password;
            sdrThreads.Value = Properties.Settings.Default.Threads;

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
            Properties.Settings.Default.StratumURL = txtStratum.Text;
            Properties.Settings.Default.Username = txtUsername.Text;
            Properties.Settings.Default.Password = txtPassword.Text;
            Properties.Settings.Default.Threads = sdrThreads.Value;

            Properties.Settings.Default.Save();

            Properties.Settings.Default.Reload();

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
