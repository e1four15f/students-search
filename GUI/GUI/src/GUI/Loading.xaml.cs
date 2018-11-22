using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        private Thread loading_thread;
        //private static ObservableCollection<string> loading_info;
        public Loading()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //loading_info = new ObservableCollection<string>();
            //LoadingInfo.ItemsSource = loading_info;
        }
        /*
        public static void AddLoadingInfo(string message)
        {
            Action action = () =>
            {
                loading_info.Add(DateTime.Now.ToLongTimeString() + " : " + message);
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(action);
        }
        */
        public void Start()
        {
            StartLoadingThread();
            this.Show();
            this.Closed += (sender, e) => this.Dispatcher.InvokeShutdown();
            Dispatcher.Run();
        }

        private void Animate()
        {
            for (int i = 0; ; i += 30)
            {
                try
                {
                    Dispatcher.Invoke(() => Loader.RenderTransform = new RotateTransform(i));
                    Thread.Sleep(50);
                }
                catch (TaskCanceledException) 
                {
                    loading_thread.Abort();
                    break;
                }
            }
        }

        private void StartLoadingThread()
        {
            loading_thread = new Thread(Animate);
            loading_thread.IsBackground = true;
            loading_thread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            Environment.Exit(0);
            base.OnClosed(e);
        }
    }
}
