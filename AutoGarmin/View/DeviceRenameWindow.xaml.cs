using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoGarmin
{
    public partial class DeviceRenameWindow : Window
    {
        private View.UserControlDevices.Device device;

        public DeviceRenameWindow(ref View.UserControlDevices.Device device)
        {
            InitializeComponent();
            TextBoxNickname.Text = device.nickname;
            this.device = device;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            device.nickname = TextBoxNickname.Text;
            this.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
