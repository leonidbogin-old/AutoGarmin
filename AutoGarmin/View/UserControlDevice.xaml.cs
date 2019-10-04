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
            LabelModel.Content = device.model + " (" + device.diskname + ")";
            LabelNickname.Content = device.nickname;
            LabelId.Content = device.id;
            LabelTimeConnect.Content = device.timeConnect.ToString("HH:mm:ss");
            ImageDevice.Source = LoadIco(device.diskname);
        }

        private BitmapImage LoadIco(string diskname)
        {
            BitmapImage bm1 = new BitmapImage();

            string path = diskname + @"\Garmin\Garmintriangletm.ico";
            if (!File.Exists(path))
            {
                List<string> files = new List<string>();
                try
                {
                    GetAllFiles(diskname + "/", "*.ico", files);
                }
                catch (UnauthorizedAccessException) { }
                if (files.Count > 0)
                    path = files[0];
                else path = "pack://application:,,,/no.ico";
            }
            bm1.BeginInit();
            bm1.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bm1.CacheOption = BitmapCacheOption.OnLoad;
            bm1.EndInit();

            return bm1;
        }

        private static void GetAllFiles(string rootDirectory, string fileExtension, List<string> files)
        {
            string[] directories = Directory.GetDirectories(rootDirectory);
            files.AddRange(Directory.GetFiles(rootDirectory, fileExtension));

            foreach (string path in directories)
                GetAllFiles(path, fileExtension, files);
        }
    }
}
