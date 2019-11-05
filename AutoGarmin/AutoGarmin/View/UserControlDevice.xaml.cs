using AutoGarmin.Class;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для UserControlDevice.xaml
    /// </summary>
    public partial class UserControlDevice : UserControl
    {
        public DeviceInfo deviceInfo;

        public UserControlDevice(DeviceInfo deviceInfo)
        {
            this.deviceInfo = deviceInfo;
            InitializeComponent();

            LabelModel.Content = $"{deviceInfo.model} ({deviceInfo.diskname})"; 
            if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0) 
                LabelNickname.Content = deviceInfo.nickname;
            LabelId.Content = deviceInfo.id; 
            LabelTimeConnect.Content = deviceInfo.timeConnect.ToString(Const.Time.Connect); 
            ImageDevice.Source = LoadIco(deviceInfo.diskname, deviceInfo.icon); 
        }

        private BitmapImage LoadIco(string diskname, string icon) //loading device icon
        {
            BitmapImage bm1 = new BitmapImage();

            string path = diskname + icon;
            if (!File.Exists(path)) path = Const.Path.NoIco;

            bm1.BeginInit();
            bm1.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bm1.CacheOption = BitmapCacheOption.OnLoad;
            bm1.EndInit();

            return bm1;
        }
    }
}
