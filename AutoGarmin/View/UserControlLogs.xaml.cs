using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoGarmin.View
{
    /// <summary>
    /// Логика взаимодействия для UserControlLogs.xaml
    /// </summary>
    public partial class UserControlLogs : UserControl
    {
        private Logs logs;

        public void LogAdd(string nickname, string diskname, string model, string action)
        {
            logs.Add("1-2", "F", "Garmin GPSMAP S66", "Устройство подключено");
            DataGridLogs.ScrollIntoView(logs.Last());
        }

        public void LogClear()
        {
            logs.Clear();
        }

        public UserControlLogs()
        {
            InitializeComponent();
            logs = new Logs();
            DataGridLogs.ItemsSource = logs.logLines;
        }

        private void DataGridLogsClear_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
        }
    }
}
