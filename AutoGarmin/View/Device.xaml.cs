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
    public partial class Device : UserControl
    {

        #region Links
        private View.Logs logs; 
        public DeviceInfo deviceInfo;
        public WindowDeviceRename deviceRenameWindow;

        #endregion

        public Device(ref DeviceInfo deviceInfo, ref View.Logs logs) //Инилизация
        {
            this.logs = logs;
            this.deviceInfo = deviceInfo;
            InitializeComponent();
            LabelModel.Content = $"{deviceInfo.model} ({deviceInfo.diskname})";
            if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0)
                LabelNickname.Content = deviceInfo.nickname;
            else LabelNickname.Content = Const.Label.NoNickname;
            LabelId.Content = deviceInfo.id;
            LabelTimeConnect.Content = deviceInfo.timeConnect.ToString(Const.Time.Connect);
            ImageDevice.Source = LoadIco(deviceInfo.diskname);
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
            logs.Add(deviceInfo, Const.Log.DeviceStartAuto);
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
            deviceInfo.ready = true;
            if (Properties.Settings.Default.SoundReady) Sound.Play(Const.Path.Sound.Ready);
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate {
                RectangleDeviceStatus.Fill = Const.Color.Device.DeviceReady();
                logs.Add(deviceInfo, Const.Log.DeviceEndAuto);
            });
        }

        public void CopyTracks()
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.GPXPath))
                {
                    string pathTarget = Const.Path.GPX + @"\" + DateTime.Now.ToShortDateString()
                        + @"\(" + DateTime.Now.ToString(Const.Time.Folder) + ")";
                    if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0)
                        pathTarget += $" - ({deviceInfo.nickname})";
                    if (deviceInfo.model != null && deviceInfo.model.Length > 0)
                        pathTarget += $" - ({deviceInfo.model})";
                    pathTarget += $" - ({deviceInfo.id})";
                    DirectoryInfo dirSource = new DirectoryInfo(deviceInfo.diskname + Const.Path.GPXPath);
                    DirectoryInfo dirTrarget = new DirectoryInfo(pathTarget);
                    FilesWork.Folder.Copy(dirSource, dirTrarget);
                    logs.Add(deviceInfo, Const.Log.DeviceTracksDownload);
                }
                else
                {
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoTracksFolder);
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceTracksDownload, Const.Color.Log.Error());
            }
        }


        public void CleanTracks()
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.GPXPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(deviceInfo.diskname + Const.Path.GPXPath);
                    FilesWork.Folder.Clean(dir);
                    logs.Add(deviceInfo, Const.Log.DeviceTracksClean);
                }
                else
                {
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoTracksFolderClean, Const.Color.Log.Error());
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceTracksClean, Const.Color.Log.Error());
            }
        }

        public void CleanMaps()
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.CustomMapsPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(deviceInfo.diskname + Const.Path.CustomMapsPath);
                    FilesWork.Folder.Clean(dir);
                    logs.Add(deviceInfo, Const.Log.DeviceMapsClean);
                }
                else
                {
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoMapsFolder, Const.Color.Log.Error());
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceMapsClean, Const.Color.Log.Error());
            }
        }

        public void LoadMap()
        {
            try
            {
                if (File.Exists(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName))
                {
                    if (!Directory.Exists(deviceInfo.diskname + Const.Path.CustomMapsPath))
                        Directory.CreateDirectory(deviceInfo.diskname + Const.Path.CustomMapsPath);
                    File.Copy(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName,
                        deviceInfo.diskname + Const.Path.CustomMapsPath + @"\" + Properties.Settings.Default.MapName, true);
                    logs.Add(deviceInfo, Const.Log.DeviceMapLoad);
                }
                else
                {
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoMapFile, Const.Color.Log.Error());
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceMapLoad, Const.Color.Log.Error());
            }
        }

        public void DeleteDevice()
        {
            deviceInfo.extract = true;
            if (deviceRenameWindow != null)
                if (deviceRenameWindow.IsVisible)
                    deviceRenameWindow.Close();
        }

        private void RenameDevice() //Переименовать устройство
        {
            try
            {
                deviceRenameWindow = new WindowDeviceRename(deviceInfo.nickname);
                if (deviceRenameWindow.ShowDialog().Value)
                {
                    if (!deviceInfo.extract && File.Exists(deviceInfo.diskname + Const.Path.GarminXml)) //if (deviceInfo.userControl == null) -> show error
                    {
                        //deviceInfo.diskname + Const.Path.GarminXml
                        //System.Windows.Forms.Application.StartupPath
                        string tempGarminXml = System.Windows.Forms.Application.StartupPath
                            + @"\" + System.IO.Path.GetFileName(Const.Path.GarminXml);
                        File.Copy(deviceInfo.diskname + Const.Path.GarminXml, tempGarminXml, true);
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(tempGarminXml);
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
                        string newLog;
                        if (deviceRenameWindow.nickname != null && deviceRenameWindow.nickname.Length > 0)
                        {
                            if (exits)
                            {
                                xmlElement.InnerText = deviceRenameWindow.nickname;
                                newLog = Const.Log.DeviceRename + deviceRenameWindow.nickname + ".";
                                LabelNickname.Content = deviceRenameWindow.nickname;
                            }
                            else
                            {
                                XmlElement xmlElem = xDoc.CreateElement(Const.Xml.Nickname, xRoot.NamespaceURI);
                                xmlElem.InnerText = deviceRenameWindow.nickname;
                                LabelNickname.Content = deviceRenameWindow.nickname;
                                xRoot.AppendChild(xmlElem);
                                newLog = Const.Log.DeviceNicknameAdd + deviceRenameWindow.nickname + ".";
                            }
                        }
                        else
                        {
                            if (exits) xRoot.RemoveChild(xmlElement);
                            LabelNickname.Content = Const.Label.NoNickname;
                            newLog = Const.Log.DeviceNicknameDelete;
                        }
                        xDoc.Save(tempGarminXml);
                        File.Copy(tempGarminXml, deviceInfo.diskname + Const.Path.GarminXml, true);
                        File.Delete(tempGarminXml);
                        logs.Add(deviceInfo, newLog);
                        deviceInfo.nickname = deviceRenameWindow.nickname;
                    }
                    else
                    {
                        //if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                        //logs.Add("Ошибка изменения наименования. Устройство было отключено");
                        //MessageBox.Show("Устройство было отключено. Изменение наименования не возможно.", "Ошибка",
                        //    MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.RenameDevice, Const.Color.Log.Error());
            }
        }

        private void DataGridDeviceRename_Click(object sender, RoutedEventArgs e) 
        {
            if (!deviceInfo.extract) RenameDevice();
        }

        private void LabelNickname_MouseDoubleClick(object sender, MouseButtonEventArgs e) 
        {
            if (!deviceInfo.extract) RenameDevice();
        }

        private void DataGridDeviceAuto_Click(object sender, RoutedEventArgs e)
        {
            StartAuto();
        }

        private void DataGridDeviceFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", deviceInfo.diskname);
        }
    }
}
