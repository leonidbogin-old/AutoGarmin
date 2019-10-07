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
    public partial class UserControlLogs : UserControl
    {
        //Коллекция логов. Привязка к dataGridLogs
        private ObservableCollection<LogLine> logLines = new ObservableCollection<LogLine>();

        public UserControlLogs() //Инициализация
        {
            InitializeComponent();
            DataGridLogs.ItemsSource = logLines;
        }

        public void LogAdd(Device device, string action) //Новый лог
        {
            LogLine logNew = new LogLine()
            {
                time = DateTime.Now.ToString(Const.Time.Log),
                id = device.id,
                nickname = device.nickname,
                modelAndDiskname = $"{device.model} ({device.diskname})",
                action = action
            };
            logLines.Add(logNew);

            DataGridLogs.ScrollIntoView(logLines.Last());
        }

        public void LogClear() //Очистка лога
        {
            logLines.Clear();
        }

        private void DataGridLogsClear_Click(object sender, RoutedEventArgs e)
        {
            LogClear();
        }
    }
}
