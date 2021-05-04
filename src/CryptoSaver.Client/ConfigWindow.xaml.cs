using CryptoSaver.Client.Properties;
using System;
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
        }

        private void SaveChanges_Action(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.StratumURL = txtStratum.Text;
            Properties.Settings.Default.Username = txtUsername.Text;
            Properties.Settings.Default.Password = txtPassword.Text;

            Properties.Settings.Default.Save();

            Properties.Settings.Default.Reload();

            this.Close();
        }
    }
}
