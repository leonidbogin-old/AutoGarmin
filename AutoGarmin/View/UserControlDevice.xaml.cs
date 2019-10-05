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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AutoGarmin
{

    public partial class UserControlDevice : UserControl
    {
        //Ссылка
        private View.UserControlLogs userControlLogs;
        //Ссылка
        public View.UserControlDevices.Device device;

        public UserControlDevice(View.UserControlDevices.Device device, View.UserControlLogs userControlLogs)
        {
            this.userControlLogs = userControlLogs;
            InitializeComponent();
            this.Name = "userControlDevice" + device.id;
            this.device = device;
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

        private void DataGridDeviceRename_Click(object sender, RoutedEventArgs e)
        {
            if (device != null)
            {
                string nickname = device.nickname;
                DeviceRenameWindow deviceRenameWindow = new DeviceRenameWindow(nickname);
                
                if (deviceRenameWindow.ShowDialog().Value)
                {
                    if (device != null)
                    {
                        device.nickname = deviceRenameWindow.nickname;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(device.diskname + @"\Garmin\GarminDevice.xml");
                        XmlElement xRoot = xDoc.DocumentElement;
                        XmlElement xmlElem = xDoc.CreateElement("Nickname", xRoot.NamespaceURI);


                        XmlText xmlText = xDoc.CreateTextNode(device.nickname);
                        xmlElem.AppendChild(xmlText);
                        xmlElem.Attributes.RemoveAll();
                        xRoot.AppendChild(xmlElem);
                        xDoc.Save(device.diskname + @"\Garmin\GarminDevice.xml");
                        
                        userControlLogs.LogAdd(device.id, device.nickname, device.diskname, device.model,
                                "Изменено наименование с '" + LabelNickname.Content + "' на '" + device.nickname + "'");
                        LabelNickname.Content = device.nickname;
                    }
                    else
                    {
                        userControlLogs.LogAdd("", "", "", "", "Ошибка изменения наименования. Устройство было отключено");
                        MessageBox.Show("Устройство было отключено. Изменение наименования не возможно.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
