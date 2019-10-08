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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AutoGarmin
{
    public partial class DeviceControl : UserControl
    {

        #region Links
        private View.Logs logs; 
        private Device device;
        #endregion

        public DeviceControl(ref Device device, ref View.Logs logs) //Инилизация
        {
            this.logs = logs;
            this.device = device;
            InitializeComponent();
            LabelModel.Content = $"{device.model} ({device.diskname})";
            if (device.nickname != null && device.nickname.Length > 0)
                LabelNickname.Content = device.nickname;
            else LabelNickname.Content = Const.Label.NoNickname;
            LabelId.Content = device.id;
            LabelTimeConnect.Content = device.timeConnect.ToString(Const.Time.Connect);
            ImageDevice.Source = LoadIco(device.diskname);
        }

        #region LoadIco
        private BitmapImage LoadIco(string diskname) //Загрузка иконки устройства
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

        private static void GetAllFiles(string rootDirectory, string fileExtension, List<string> files) //Поиск .ico файлов на устройстве
        {
            string[] directories = Directory.GetDirectories(rootDirectory);
            files.AddRange(Directory.GetFiles(rootDirectory, fileExtension));

            foreach (string path in directories)
                GetAllFiles(path, fileExtension, files);
        }
        #endregion

        public void StartAuto()
        {
            System.Threading.Thread myThread = new System.Threading.Thread(
                                new System.Threading.ThreadStart(Auto));
            myThread.Start();
        }

        private void Auto()
        {
            if (Properties.Settings.Default.AutoTracksDownload)
            {
                CopyTracks();
            }
            if (Properties.Settings.Default.AutoTracksClean)
            {
                CleanTracks();
            }
            if (Properties.Settings.Default.AutoMapClean)
            {
                CleanMaps();
            }
            if (Properties.Settings.Default.AutoMapLoad)
            {
                LoadMap();
            }
            device.ready = true;
            if (Properties.Settings.Default.SoundReady)
                Sound.Play(Const.Path.Sound.Ready);
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate {
                device.control.RectangleDeviceStatus.Fill = Const.Color.DeviceReady();
            });
        }

        public void CopyTracks()
        {

            if (Directory.Exists(device.diskname + Const.Path.GPXPath))
            {
                string pathTarget = Const.Path.GPX + @"\" + DateTime.Now.ToShortDateString()
                    + @"\(" + DateTime.Now.ToString(Const.Time.Folder) + ")";
                if (device.nickname != null && device.nickname.Length > 0)
                    pathTarget += $" - ({device.nickname})";
                if (device.model != null && device.model.Length > 0)
                    pathTarget += $" - ({device.model})";
                pathTarget += $" - ({device.id})";
                DirectoryInfo dirSource = new DirectoryInfo(device.diskname + Const.Path.GPXPath);
                DirectoryInfo dirTrarget = new DirectoryInfo(pathTarget);
                CopyAll(dirSource, dirTrarget);
            }
            else
            {
                if (Properties.Settings.Default.SoundError)
                    Sound.Play(Const.Path.Sound.Error);
                logs.Add(device, Const.Error.NoTrack);
            }
        }


        public void CleanTracks()
        {
            if (Directory.Exists(device.diskname + Const.Path.GPXPath))
            {
                DirectoryInfo dir = new DirectoryInfo(device.diskname + Const.Path.GPXPath);
                DeleteAll(dir);
            }
        }

        public void CleanMaps()
        {
            if (Directory.Exists(device.diskname + Const.Path.CustomMapsPath))
            {
                DirectoryInfo dir = new DirectoryInfo(device.diskname + Const.Path.CustomMapsPath);
                DeleteAll(dir);
            }
        }

        public void LoadMap()
        {
            if (Directory.Exists(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName))
            {
                File.Copy(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName,
                    device.diskname + Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName, true);
            }
            else
            {
                if (Properties.Settings.Default.SoundError)
                    Sound.Play(Const.Path.Sound.Error);
                logs.Add(device, Const.Error.NoLoad);
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target) //Копирование папки
        {
            // Если директория для копирования файлов не существует, то создаем ее
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Копируем все файлы в новую директорию
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
            }

            // Копируем рекурсивно все поддиректории
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                // Создаем новую поддиректорию в директории
                DirectoryInfo nextTargetSubDir =
                  target.CreateSubdirectory(diSourceSubDir.Name);
                // Опять вызываем функцию копирования
                // Рекурсия
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void DeleteAll(DirectoryInfo source) //Копирование папки
        {
            foreach (FileInfo file in source.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void RenameDevice() //Переименовать устройство
        {
            if (device.control != null) //if (device.userControl == null) -> return
            {
                string nickname = device.nickname;
                WindowDeviceRename deviceRenameWindow = new WindowDeviceRename(nickname);
                if (deviceRenameWindow.ShowDialog().Value)
                {
                    if (device.control != null) //if (device.userControl == null) -> show error
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
                        if (device.nickname != null && device.nickname.Length > 0)
                        {
                            if (exits)
                            {
                                xmlElement.InnerText = device.nickname;
                                LabelNickname.Content = device.nickname;
                                logs.Add(device, $"Изменено наименование с '{LabelNickname.Content}' на '{device.nickname}'");
                            }
                            else
                            {
                                XmlElement xmlElem = xDoc.CreateElement(Const.Xml.Nickname, xRoot.NamespaceURI);
                                xmlElem.InnerText = device.nickname;
                                LabelNickname.Content = device.nickname;
                                xRoot.AppendChild(xmlElem);
                                logs.Add(device, $"Добавлено наименование '{device.nickname}'");
                            }
                        }
                        else
                        { 
                            if (exits) xRoot.RemoveChild(xmlElement);
                            LabelNickname.Content = Const.Label.NoNickname;
                            logs.Add(device, $"Удалено наименование");
                        }
                        xDoc.Save(device.diskname + Const.Path.GarminXml);
                    }
                    else
                    {
                        logs.Add(new Device(), "Ошибка изменения наименования. Устройство было отключено");
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

        private void DataGridDeviceAuto_Click(object sender, RoutedEventArgs e)
        {
            StartAuto();
        }
    }
}
