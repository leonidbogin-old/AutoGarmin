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
        //Информация об устройстве
        public DeviceInfo deviceInfo;

        //Инициализация
        public UserControlDevice(DeviceInfo deviceInfo)
        {
            this.deviceInfo = deviceInfo;
            InitializeComponent();
            ProgressBarDevice.Value = 0;

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
            else
            {
                deviceInfo.nickname = "";
                LabelNickname.Content = Const.Label.NoNickname;
                LabelNickname.ToolTip = Const.Label.NoNicknameTooltip;
            }
            LabelId.Content = deviceInfo.id; 
            LabelTimeConnect.Content = deviceInfo.timeConnect.ToString(Const.Time.Connect); 
            ImageDevice.Source = LoadIco(deviceInfo.diskname, deviceInfo.icon);

            if (Properties.Settings.Default.UploadMapScript)
            {
                ProgressBarDevice.Value = 20;
                System.Threading.Thread myThread = new System.Threading.Thread(
                    new System.Threading.ThreadStart(MapUploadScript));
                myThread.Start();
            }
        }

        //Загрузка иконки
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

        //Переименовать устройство
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
                if (Directory.Exists(deviceInfo.diskname))
                {
                    try
                    {
                        xDocInfo.Save(deviceInfo.diskname + Const.Path.GarminInformationXml);
                    }
                    finally
                    {
                        if (windowDeviceRename.Result != null && windowDeviceRename.Result.Length > 0)
                        {
                            if (windowDeviceRename.Result.Length > 20)
                            {
                                LabelNickname.ToolTip = windowDeviceRename.Result;
                                LabelNickname.Content = windowDeviceRename.Result.Substring(0, 20) + "..";
                            }
                            else
                            {
                                LabelNickname.Content = windowDeviceRename.Result;
                                LabelNickname.ToolTip = "";
                            }
                            deviceInfo.nickname = windowDeviceRename.Result;
                        }
                        else
                        {
                            deviceInfo.nickname = "";
                            LabelNickname.Content = Const.Label.NoNickname;
                            LabelNickname.ToolTip = Const.Label.NoNicknameTooltip;
                        }
                    }

                }
            }
        }

        //Скрипт загрузки карты
        public void MapUploadScript()
        {
            if (Properties.Settings.Default.RemoveOldMap)
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.CustomMapsPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(deviceInfo.diskname + Const.Path.CustomMapsPath);
                    FilesWork.Folder.Clean(dir);
                }
            }
            //Загрузить выбранные файлы (файлы из папки program/CustomMaps/) ----------------------------------------------------------------------
        }
    }
}
