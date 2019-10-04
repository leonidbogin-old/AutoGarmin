using AutoGarmin.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoGarmin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region View

        private UserControlDevices userControlDevices;
        private UserControlLogs userControlLogs;
        
        #endregion


        ObservableCollection<Log> logs = null;


        public MainWindow()
        {
            InitializeComponent();
            userControlDevices = new UserControlDevices();
            userControlLogs = new UserControlLogs();

            userControlDevices.Visibility = Visibility.Visible;
            userControlLogs.Visibility = Visibility.Hidden;

            GridContent.Children.Add(userControlDevices);
            GridContent.Children.Add(userControlLogs);
        }

        private class Log
        {
            public string time { get; set; }
            public string nickname { get; set; }
            public string model { get; set; }
            public string action { get; set; }
        }

        //private int i = 0;

        //if (logs == null)
        //{
        //    logs = new ObservableCollection<Log>();
        //    DataGridLogs.ItemsSource = logs;
        //}
        //Log log = new Log() { time = DateTime.Now.ToString("HH:mm:ss"), nickname = "1-1", model = "Garmin GPSMAP 66S", action = "Устройство подключено" };
        //logs.Add(log);
        //DataGridLogs.ScrollIntoView(log);

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    userControlDevices.StackPanelDevices.Children.Add(new UserControlDevice());
        //    userControlDevices.ScrollViewerDevices.ScrollToBottom();
        //}

        private void ButtonDevices_Click(object sender, RoutedEventArgs e)
        {
            if (userControlDevices.Visibility == Visibility.Hidden)
            {
                userControlDevices.Visibility = Visibility.Visible;
                userControlLogs.Visibility = Visibility.Hidden;
                ButtonLogs.Style = (Style)FindResource("MainButton");
                ButtonDevices.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonLogs_Click(object sender, RoutedEventArgs e)
        {
            if (userControlLogs.Visibility == Visibility.Hidden)
            {
                userControlLogs.Visibility = Visibility.Visible;
                userControlDevices.Visibility = Visibility.Hidden;
                ButtonDevices.Style = (Style)FindResource("MainButton");
                ButtonLogs.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonTrackFolder_Click(object sender, RoutedEventArgs e)
        {
            Process Proc = new Process();
            Proc.StartInfo.FileName = "explorer";
            Proc.StartInfo.Arguments = "GPX";
            Proc.Start();
            Proc.Close();
        }
    }
}
