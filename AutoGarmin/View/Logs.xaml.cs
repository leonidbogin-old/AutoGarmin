using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public partial class Logs : UserControl
    {
        //Collection of logs. Binding to dataGridLogs
        private ObservableCollection<LogLine> logLines = new ObservableCollection<LogLine>();

        public Logs() //start
        {
            InitializeComponent();
            DataGridLogs.ItemsSource = logLines; //Binding
        }

        public void Add(DeviceInfo deviceInfo, string action) //New log entry
        {
            LogLine logNew = new LogLine()
            {
                time = DateTime.Now.ToString(Const.Time.Log),
                id = deviceInfo.id,
                nickname = deviceInfo.nickname,
                modelAndDiskname = $"{deviceInfo.model} ({deviceInfo.diskname})",
                action = action
            };
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate {
                logLines.Add(logNew);
                DataGridLogs.ScrollIntoView(logLines.Last());
            });
        }

        public void Clear() //Clearing the log
        {
            logLines.Clear();
        }

        private void DataGridLogsClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
    }


}
