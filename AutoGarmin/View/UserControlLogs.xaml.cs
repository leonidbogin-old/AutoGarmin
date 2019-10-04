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

        #region Logs

        private Logs logs;

        public void LogAdd(string id, string nickname, string diskname, string model, string action)
        {
            logs.Add(id, nickname, diskname, model, action);
            DataGridLogs.ScrollIntoView(logs.Last());
        }

        public void LogClear()
        {
            logs.Clear();
        }

        #endregion

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
