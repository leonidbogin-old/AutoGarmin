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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoGarmin
{
    /// <summary>
    /// Логика взаимодействия для UserControlDevice.xaml
    /// </summary>
    public partial class UserControlDevice : UserControl
    {
        public UserControlDevice(string Name, View.UserControlDevices.Device device)
        {
            InitializeComponent();
            this.Name = Name;
            LabelModel.ToolTip = "Device id: " + device.id;
            LabelModel.Content = device.model + " (" + device.diskname + ")";
            LabelNickname.Content = device.nickname;
        }
    }
}
