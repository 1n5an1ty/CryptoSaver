using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        private readonly Random _randomGen;
        private readonly Timer _timer;
        private readonly double _logoWidth;
        private readonly double _logoHeight;


        public MainWindow()
        {
            InitializeComponent();
            _randomGen = new Random();
            _timer = new Timer(ReposistionElement, null, 0, Timeout.Infinite);

            var win = this.GetCurrentScreenWorkArea();
            //txtMessage.Width = win.Width * .25;
            //txtMessage.Height = win.Height * .25;
            _logoWidth = txtMessage.Width;
            _logoHeight = txtMessage.Height;
        }

        private void ReposistionElement(object state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var win = this.GetCurrentScreenWorkArea();
                var bottomBounds = win.Height > _logoHeight ? win.Height - _logoHeight : win.Height;
                var rightBounds = win.Width > _logoWidth ? win.Width - _logoWidth : win.Width;

                Canvas.SetLeft(txtMessage, _randomGen.Next((int)rightBounds));
                Canvas.SetTop(txtMessage, _randomGen.Next((int)bottomBounds));
            });


            _timer.Change(2333, Timeout.Infinite);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Log().Info("CryptoSaver is starting");
            
            //var startPoint = 1;
            //while (true)
            //{
            //    //Canvas.SetLeft(txtMessage, rand.Next(0, (int)this.ActualWidth));
            //    //Canvas.SetTop(txtMessage, rand.Next(0, (int)this.ActualHeight));

            //    //MoveText(startPoint * 1.33);
            //    //Thread.Sleep(1000);
            //    startPoint++;
            //}
        }

        private void MoveText(double start)
        {
            //txtMessage.SetValue(TextBlock., start);
        }
    }
}
