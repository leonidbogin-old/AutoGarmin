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
using System.Xml;

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

            string model = deviceInfo.model;
            if (model.Length > 16)
            {
                LabelModel.ToolTip = model;
                model = model.Substring(0, 20) + "..";
            }

            LabelModel.Content = $"{model} ({deviceInfo.diskname})";
            if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0)
            {
                if (deviceInfo.nickname.Length > 20)
                {
                    LabelNickname.ToolTip = deviceInfo.nickname;
                    LabelNickname.Content = deviceInfo.nickname.Substring(0, 20) + "..";
                }
                else LabelNickname.Content = deviceInfo.nickname;
            }
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

        private void LabelNickname_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowDeviceRename windowDeviceRename = new WindowDeviceRename(deviceInfo.nickname);
            if (windowDeviceRename.ShowDialog().Value)
            {
                XmlDocument xDocInfo = new XmlDocument();
                XmlElement xRootInfo;
                if (File.Exists(deviceInfo.diskname + Const.Path.GarminInformationXml))
                {
                    xDocInfo.Load(deviceInfo.diskname + Const.Path.GarminInformationXml);
                    xRootInfo = xDocInfo.DocumentElement;
                }
                else
                {
                    xRootInfo = xDocInfo.CreateElement(Const.Xml.GarminInformation);
                    xDocInfo.AppendChild(xRootInfo);
                }
                
                bool find = false;
                foreach (XmlElement xmlElement in xRootInfo)
                {
                    if (xmlElement.Name == Const.Xml.Nickname)
                    {
                        xmlElement.InnerText = windowDeviceRename.Result;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    XmlElement xmlElem = xDocInfo.CreateElement(Const.Xml.Nickname, xRootInfo.NamespaceURI);
                    xmlElem.InnerText = windowDeviceRename.Result;
                    xRootInfo.AppendChild(xmlElem);
                }
                if (File.Exists(deviceInfo.diskname + Const.Path.GarminInformationXml))
                {
                    try
                    {
                        xDocInfo.Save(deviceInfo.diskname + Const.Path.GarminInformationXml);
                    }
                    finally
                    {
                        if (windowDeviceRename.Result != null && windowDeviceRename.Result.Length > 0)
                        {
                            if (deviceInfo.nickname.Length > 20)
                            {
                                LabelNickname.ToolTip = deviceInfo.nickname;
                                LabelNickname.Content = deviceInfo.nickname.Substring(0, 20) + "..";
                            }
                            else LabelNickname.Content = deviceInfo.nickname;
                        }
                        else LabelNickname.Content = Const.Label.NoNickname;
                        deviceInfo.nickname = windowDeviceRename.Result;
                    }
                }
            }
        }
    }
}
