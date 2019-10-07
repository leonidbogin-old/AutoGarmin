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
        private View.UserControlLogs logs; //Ссылка
        private Device device; //Ссылка

        public UserControlDevice(ref Device device, ref View.UserControlLogs logs)
        {
            this.logs = logs;
            this.device = device;
            InitializeComponent();
            this.Name = "userControlDevice" + device.id; //UI Id
            LabelModel.Content = $"{device.model} ({device.diskname})";
            if (device.nickname != null && device.nickname.Length > 0)
                LabelNickname.Content = device.nickname;
            else LabelNickname.Content = Const.Label.NoNickname;
            LabelId.Content = device.id;
            LabelTimeConnect.Content = device.timeConnect.ToString("HH:mm:ss");
            ImageDevice.Source = LoadIco(device.diskname);
        }

        #region LoadIco
        private BitmapImage LoadIco(string diskname)
        {
            BitmapImage bm1 = new BitmapImage();

            string path = diskname + Const.Path.GarminIco;
            if (!File.Exists(path))
            {
                //Если нет стандартного ico, ищем любые другие ico на устройстве
                List<string> files = new List<string>();
                try
                {
                    GetAllFiles(diskname + "/", "*.ico", files);
                }
                catch (UnauthorizedAccessException) { }
                if (files.Count > 0)
                    path = files[0];
                else path = Const.Path.NoIco; //Иначе ставим картинку из ресурсов 'no.ico'
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
        #endregion

        private void RenameDevice()
        {
            if (device.userControl != null) //if (device.userControl == null) -> return
            {
                string nickname = device.nickname;
                WindowDeviceRename deviceRenameWindow = new WindowDeviceRename(nickname);
                if (deviceRenameWindow.ShowDialog().Value)
                {
                    if (device.userControl != null) //if (device.userControl == null) -> show error
                    {
                        device.nickname = deviceRenameWindow.nickname;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(device.diskname + Const.Path.GarminXml); 
                        XmlElement xRoot = xDoc.DocumentElement;
                        bool exits = false;
                        XmlElement xmlElement = null;
                        foreach (XmlElement xmlElementFind in xRoot)
                        {
                            if (xmlElementFind.Name == Const.Xml.Nickname)
                            {
                                xmlElement = xmlElementFind;
                                exits = true;
                                break;
                            }
                        }
                        //if (xmlElementRemove != null) xRoot.RemoveChild(xmlElementRemove);
                        //if (device.nickname != null && device.nickname.Length > 0)
                        //{
                        //    xmlElement.InnerText = device.nickname;
                        //    LabelNickname.Content = device.nickname;
                        //    logs.LogAdd(device, $"Изменено наименование с '{LabelNickname.Content}' на '{device.nickname}'");
                        //}
                        //else
                        //{
                        //    xmlElementRemove = xmlElement;
                        //    LabelNickname.Content = Const.Label.NoNickname;
                        //    logs.LogAdd(device, $"Удалено наименование");
                        //}

                        if (device.nickname != null && device.nickname.Length > 0)
                        {
                            if (exits)
                            {
                                xmlElement.InnerText = device.nickname;
                                LabelNickname.Content = device.nickname;
                                logs.LogAdd(device, $"Изменено наименование с '{LabelNickname.Content}' на '{device.nickname}'");
                            }
                            else
                            {
                                XmlElement xmlElem = xDoc.CreateElement(Const.Xml.Nickname, xRoot.NamespaceURI);
                                xmlElem.InnerText = device.nickname;
                                LabelNickname.Content = device.nickname;
                                xRoot.AppendChild(xmlElem);
                                logs.LogAdd(device, $"Добавлено наименование '{device.nickname}'");
                            }
                        }
                        else
                        { 
                            if (exits) xRoot.RemoveChild(xmlElement);
                            LabelNickname.Content = Const.Label.NoNickname;
                            logs.LogAdd(device, $"Удалено наименование");
                        }
                        xDoc.Save(device.diskname + Const.Path.GarminXml);
                    }
                    else
                    {
                        logs.LogAdd(new Device(), "Ошибка изменения наименования. Устройство было отключено");
                        MessageBox.Show("Устройство было отключено. Изменение наименования не возможно.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DataGridDeviceRename_Click(object sender, RoutedEventArgs e)
        {
            RenameDevice();
        }

        private void LabelNickname_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RenameDevice();
        }
    }
}
